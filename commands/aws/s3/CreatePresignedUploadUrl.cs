using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using CoarUtils.commands.logging;
using CoarUtils.commands.web;
using CoarUtils.models.commands;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CoarUtils.commands.aws.s3 {
  public class CreatePresignedUploadUrl {
    public static async Task<Response> Execute(
      string awsAccessKey,
      string awsSecretKey,
      string bucketName,
      string key,
      Amazon.RegionEndpoint regionEndpoint,
      CancellationToken cancellationToken,
      string contentType = null,
      HttpContext hc = null,
      int numberOfMinutes = 30
    ) {
      var response = new Response { };
      try {
        using (var s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, regionEndpoint)) {
          var gpsur = new GetPreSignedUrlRequest {
            BucketName = bucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(numberOfMinutes),
            ContentType = contentType,
          };
          response.url = await s3Client.GetPreSignedURLAsync(gpsur);
        }
        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.E(ex, cancellationToken);
        return response = new Response { status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS, httpStatusCode = HttpStatusCode.InternalServerError };
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            bucketName,
            key,
            response.url,
            numberOfMinutes,
            ipAddress = GetPublicIpAddress.Execute(hc),
            executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented), cancellationToken);
      }
    }
  }
}
