using Amazon;
using Amazon.S3;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.aws.s3 {
  public class Get {
    public class Request {
      public string bucketName { get; set; }
      public string key { get; set; }
      public RegionEndpoint re { get; set; }
    }


    public static void Execute(
      Request request,
      out MemoryStream ms,
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey,
      CancellationToken cancellationToken
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      ms = null;
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, request.re)) {
          var getObjectRequest = new Amazon.S3.Model.GetObjectRequest {
            BucketName = request.bucketName,
            Key = request.key,
          };
          var response = s3c.GetObjectAsync(getObjectRequest).Result;
          using (var rs = response.ResponseStream) {
            ms = new MemoryStream();
            rs.CopyTo(ms);
          }
        }
        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.CANCELLATION_REQUESTED_STATUS;
          return;
        }

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


    public static void Execute(
      Request request,
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey,
      out byte[] ba,
      CancellationToken cancellationToken
    ) {
      ba = null;
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, request.re)) {
          var getObjectRequest = new Amazon.S3.Model.GetObjectRequest {
            BucketName = request.bucketName,
            Key = request.key,
          };
          var response = s3c.GetObjectAsync(getObjectRequest).Result;
          using (var rs = response.ResponseStream) {
            using (var ms = new MemoryStream()) {
              rs.CopyTo(ms);
              ba = ms.ToArray();
            }
          }
        }
        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.CANCELLATION_REQUESTED_STATUS;
          return;
        }

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

    //public static void Execute(Request request, out HttpStatusCode hsc) {
    //  throw new NotImplementedException();
    //}
  }
}







