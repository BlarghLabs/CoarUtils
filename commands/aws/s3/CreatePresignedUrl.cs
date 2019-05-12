using Amazon.S3;
using Amazon.S3.Model;
using CoarUtils.commands.logging;
using CoarUtils.commands.web;
using Newtonsoft.Json;
using System;

namespace CoarUtils.commands.aws.s3 {

  //TODO: on fail behavior ... what to do?
  public class CreatePresignedUrl {
    public static bool Execute(
      string bucketName,
      string objectKey,
      out string url,
      int numberOfMinutes = 30
    ) {
      url = "";

      try {
        using (var s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1)) {
          var gpsur = new GetPreSignedUrlRequest {
            BucketName = bucketName,
            Key = objectKey,
            Expires = DateTime.UtcNow.AddMinutes(numberOfMinutes)
          };
          url = s3Client.GetPreSignedURL(gpsur);
        }
        return true;
      } catch (Exception ex) {
        LogIt.E(ex.Message);
        return false;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            bucketName,
            objectKey,
            url,
            numberOfMinutes,
            ipAddress = GetPublicIpAddress.Execute(),
            executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }

  }
}


