using System.Net;
using Amazon;
using Amazon.S3;
using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;

namespace CoarUtils.commands.aws.s3 {
  public class Get {
    #region models
    public class Request {
      public string bucketName { get; set; }
      public string key { get; set; }
      public RegionEndpoint re { get; set; }
    }
    public class ByteArrayResponse : ResponseStatusModel {
      public byte[] ba { get; set; }
    }
    #endregion

    public static async Task<ByteArrayResponse> Execute(
      Request request,
      string awsAccessKey,
      string awsSecretKey,
      CancellationToken cancellationToken
    ) {
      var response = new ByteArrayResponse { };
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, request.re)) {
          var getObjectRequest = new Amazon.S3.Model.GetObjectRequest {
            BucketName = request.bucketName,
            Key = request.key,
          };
          var getObjectAsyncResponse = await s3c.GetObjectAsync(getObjectRequest, cancellationToken).ConfigureAwait(false);
          using (var rs = getObjectAsyncResponse.ResponseStream) {
            using (var ms = new MemoryStream()) {
              rs.CopyTo(ms);
              response.ba = ms.ToArray();
            }
          }
        }
        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new ByteArrayResponse { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.E(ex, cancellationToken);
        return response = new ByteArrayResponse { status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS, httpStatusCode = HttpStatusCode.InternalServerError };
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            request,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented), cancellationToken);
      }
    }

    //public static void Execute(Request request, out HttpStatusCode hsc) {
    //  throw new NotImplementedException();
    //}
  }
}







