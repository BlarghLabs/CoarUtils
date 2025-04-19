using System.Net;
using Amazon;
using Amazon.S3;
using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;

namespace CoarUtils.commands.aws.s3 {
  public class Delete {
    #region models
    public class Request {
      public string bucketName { get; set; }
      public string key { get; set; }
      public RegionEndpoint re { get; set; }
    }
    public class Response : ResponseStatusModel { }
    #endregion

    //TODO: make sync
    public static void Execute(
    Request request,
    out HttpStatusCode hsc,
    out string status,
    string awsAccessKey,
    string awsSecretKey,
    CancellationToken cancellationToken
  ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, request.re)) {
          var deleteObjectRequest = new Amazon.S3.Model.DeleteObjectRequest {
            BucketName = request.bucketName,
            Key = request.key,
          };
          var dor = s3c.DeleteObjectAsync(deleteObjectRequest).Result;
          hsc = dor.HttpStatusCode == System.Net.HttpStatusCode.NoContent
            ? HttpStatusCode.OK
            : HttpStatusCode.BadRequest
          ;
        }
        return;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        LogIt.I(ex, cancellationToken);
        hsc = HttpStatusCode.InternalServerError;
        status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS;
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            request,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented), cancellationToken);
      }
    }

    public static async Task<Response> Execute(
      Request request,
      string awsAccessKey,
      string awsSecretKey,
      CancellationToken cancellationToken
    ) {
      var response = new Response(); ;
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, request.re)) {
          var deleteObjectRequest = new Amazon.S3.Model.DeleteObjectRequest {
            BucketName = request.bucketName,
            Key = request.key,
          };
          var dor = await s3c.DeleteObjectAsync(deleteObjectRequest);
          response.httpStatusCode = dor.HttpStatusCode == System.Net.HttpStatusCode.NoContent
            ? HttpStatusCode.OK
            : HttpStatusCode.BadRequest
          ;
        }
        return response;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.I(ex, cancellationToken);
        return response = new Response { httpStatusCode = HttpStatusCode.InternalServerError, status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS };
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          response.httpStatusCode,
          response.status,
          request,
          //UserService.atmospherics,
        }, Formatting.Indented), cancellationToken);
      }
    }
  }
}



