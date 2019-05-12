using Amazon.S3;
using CoarUtils.lib;

namespace CoarUtils.commands.aws.s3 {

  //TODO: add logging
  public class Exists {
    public static bool Execute(
      string awsAccessKey,
      string awsSecretKey,
      Amazon.RegionEndpoint re,
      string key,
      string bucketName,
      out string url
    //CancellationToken ct
    ) {
      url = "";
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, re)) {
          var gomr = s3c.GetObjectMetadataAsync(request: new Amazon.S3.Model.GetObjectMetadataRequest {
            BucketName = bucketName,
            Key = key
          }).Result;
          url = Constants.S3_BASE + bucketName + "/" + key;
          return gomr.ContentLength != 0;
          //return true;
        }
      } catch (Amazon.S3.AmazonS3Exception ex) {
        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
          return false;
        }

        //status wasn't not found, so throw the exception
        throw;
      }
    }

    public static bool Execute(
      string awsAccessKey,
      string awsSecretKey,
      Amazon.RegionEndpoint re,
      string key,
      string bucketName,
      out long contentLength
    //CancellationToken ct
    ) {
      contentLength = 0;
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, re)) {
          var gomr = s3c.GetObjectMetadataAsync(request: new Amazon.S3.Model.GetObjectMetadataRequest {
            BucketName = bucketName,
            Key = key
          }).Result;
          contentLength = gomr.ContentLength;
          return gomr.ContentLength != 0;
        }
      } catch (Amazon.S3.AmazonS3Exception ex) {
        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
          contentLength = 0;
          return false;
        }

        //?
        throw;
      }
    }

  }
}


