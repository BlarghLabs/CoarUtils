using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.aws.s3 {
  public class Copy {
    #region models
    public class Request {
      public S3CannedACL destAcl { get; set; }
      public string contentType { get; set; }
      public string destBucketName { get; set; }
      public string destKey { get; set; }

      public string sourceBucketName { get; set; }
      public string awsAccessKey { get; set; }
      public string awsSecretKey { get; set; }
      public string sourceKey { get; set; }
      public RegionEndpoint regionEndpoint { get; set; }
    }
    public class Response : models.commands.ResponseStatusModel {
    }
    #endregion

    public async static Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
      //HttpContext hc = null
    ) {
      var response = new Response { };
      try {
        if (!await Exists.Execute(
          key: request.sourceKey,
          bucketName: request.sourceBucketName,
          //url: out string sourceUrl,
          regionEndpoint: request.regionEndpoint,
          awsAccessKey: request.awsAccessKey,
          awsSecretKey: request.awsSecretKey,
          cancellationToken: cancellationToken
        )) {
          response.status = "source did not exist";
          response.httpStatusCode = HttpStatusCode.BadRequest;
          return response;
        }

        //validate dest doesn't already exist, fail if it does bc we aren't validating that it is different? maybe shar eeach in future?

        if (await Exists.Execute(
          key: request.destKey,
          bucketName: request.destBucketName,
          //url: out string destUrl,
          regionEndpoint: request.regionEndpoint,
          awsAccessKey: request.awsAccessKey,
          awsSecretKey: request.awsSecretKey,
          cancellationToken: cancellationToken
        )) {
          response.status = "dest existed already";
          response.httpStatusCode = HttpStatusCode.BadRequest;
          return response;
        }

        //copy 
        using (var amazonS3Client = new AmazonS3Client(
          awsAccessKeyId: request.awsAccessKey,
          awsSecretAccessKey: request.awsSecretKey,
          region: request.regionEndpoint
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
          var copyObjectResponse = await amazonS3Client.CopyObjectAsync(copyObjectRequest, cancellationToken: cancellationToken);
          response.httpStatusCode = copyObjectResponse.HttpStatusCode;
          return response;
          //fileLengthBytes = cor.
        }
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }

        LogIt.E(ex);
        response.httpStatusCode = HttpStatusCode.InternalServerError;
        response.status = "unexecpected error";
        return response;
      } finally {
        request.awsAccessKey = "DO_NOT_LOG";
        request.awsSecretKey = "DO_NOT_LOG";
        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            request,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}