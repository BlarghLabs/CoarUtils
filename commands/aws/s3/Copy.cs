using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading;

namespace CoarUtils.commands.aws.s3 {
  public class Copy {
    public class request {
      public S3CannedACL destAcl { get; set; }
      public string contentType { get; set; }
      public string destBucketName { get; set; }
      public string destKey { get; set; }

      public string sourceBucketName { get; set; }
      public string sourceKey { get; set; }
      public RegionEndpoint re { get; set; }
    }


    public static void Execute(
      request m,
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey,
      Microsoft.AspNetCore.Http.HttpContext hc = null, CancellationToken? ct = null
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        if (!Exists.Execute(
          key: m.sourceKey,
          bucketName: m.sourceBucketName,
          url: out string sourceUrl,
          re: m.re,
          awsAccessKey: awsAccessKey,
          awsSecretKey: awsSecretKey
        )) {
          status = "source did not exist";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        //validate dest doesn't already exist, fail if it does bc we aren't validating that it is different? maybe shar eeach in future?

        if (Exists.Execute(
          key: m.destKey,
          bucketName: m.destBucketName,
          url: out string destUrl,
          re: m.re,
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
          region: m.re
        )) {
          var request = new CopyObjectRequest {
            SourceBucket = m.sourceBucketName,
            SourceKey = m.sourceKey,
            DestinationBucket = m.destBucketName,
            DestinationKey = m.destKey,
            CannedACL = m.destAcl
          };
          if (!string.IsNullOrWhiteSpace(m.contentType)) {
            request.MetadataDirective = S3MetadataDirective.REPLACE;
            request.ContentType = m.contentType;
          }
          var response = s3c.CopyObjectAsync(request, cancellationToken: ct.HasValue ? ct.Value : CancellationToken.None).Result;
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
            m,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}