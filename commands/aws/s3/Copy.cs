using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CoarUtils.commands.logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.aws.s3 {
  public class Copy {
    public class Request {
      public S3CannedACL destAcl { get; set; }
      public string contentType { get; set; }
      public string destBucketName { get; set; }
      public string destKey { get; set; }

      public string sourceBucketName { get; set; }
      public string sourceKey { get; set; }
      public RegionEndpoint re { get; set; }
    }


    public static void Execute(
      Request request,
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey,
      HttpContext hc = null, 
      CancellationToken? ct = null
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        if (!Exists.Execute(
          key: request.sourceKey,
          bucketName: request.sourceBucketName,
          url: out string sourceUrl,
          re: request.re,
          awsAccessKey: awsAccessKey,
          awsSecretKey: awsSecretKey
        )) {
          status = "source did not exist";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        //validate dest doesn't already exist, fail if it does bc we aren't validating that it is different? maybe shar eeach in future?

        if (Exists.Execute(
          key: request.destKey,
          bucketName: request.destBucketName,
          url: out string destUrl,
          re: request.re,
          awsAccessKey: awsAccessKey,
          awsSecretKey: awsSecretKey
        )) {
          status = "dest existed already";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        //copy 
        using (var s3c = new AmazonS3Client(
          awsAccessKeyId: awsAccessKey,
          awsSecretAccessKey: awsSecretKey,
          region: request.re
        )) {
          var copyObjectRequest = new CopyObjectRequest {
            SourceBucket = request.sourceBucketName,
            SourceKey = request.sourceKey,
            DestinationBucket = request.destBucketName,
            DestinationKey = request.destKey,
            CannedACL = request.destAcl
          };
          if (!string.IsNullOrWhiteSpace(request.contentType)) {
            copyObjectRequest.MetadataDirective = S3MetadataDirective.REPLACE;
            copyObjectRequest.ContentType = request.contentType;
          }
          var response = s3c.CopyObjectAsync(copyObjectRequest, cancellationToken: ct.HasValue ? ct.Value : CancellationToken.None).Result;
          hsc = response.HttpStatusCode;
          return;
          //fileLengthBytes = cor.
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = "unexecpected error";
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            request,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}