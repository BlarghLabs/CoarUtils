using Amazon;
using Amazon.S3;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace CoarUtils.commands.aws.s3 {
  public class Get {
    public class request {
      public string bucketName { get; set; }
      public string key { get; set; }
      public RegionEndpoint re { get; set; }
    }


    public static void Execute(
      request m,
      out MemoryStream ms,
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      ms = null;
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, m.re)) {
          var request = new Amazon.S3.Model.GetObjectRequest {
            BucketName = m.bucketName,
            Key = m.key,
          };
          var response = s3c.GetObjectAsync(request).Result;
          using (var rs = response.ResponseStream) {
            ms = new MemoryStream();
            rs.CopyTo(ms);
          }
        }
        hsc = HttpStatusCode.OK;
        return;
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


    public static void Execute(
      request m,
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey,
      out byte[] ba
    ) {
      ba = null;
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, m.re)) {
          var request = new Amazon.S3.Model.GetObjectRequest {
            BucketName = m.bucketName,
            Key = m.key,
          };
          var response = s3c.GetObjectAsync(request).Result;
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

    //public static void Execute(request m, out HttpStatusCode hsc) {
    //  throw new NotImplementedException();
    //}
  }
}







