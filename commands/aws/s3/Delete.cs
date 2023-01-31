using Amazon;
using Amazon.S3;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System;
using System.Net;

namespace CoarUtils.commands.aws.s3 {
  public class Delete {
    public class Request {
      public string bucketName { get; set; }
      public string key { get; set; }
      public RegionEndpoint re { get; set; }
    }


    public static void Execute(
      Request m,
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, m.re)) {
          var request = new Amazon.S3.Model.DeleteObjectRequest {
            BucketName = m.bucketName,
            Key = m.key,
          };
          var dor = s3c.DeleteObjectAsync(request).Result;
          hsc = dor.HttpStatusCode == System.Net.HttpStatusCode.NoContent
            ? HttpStatusCode.OK
            : HttpStatusCode.BadRequest
          ;
        }
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

  }
}







