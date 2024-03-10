using Amazon.S3;
using Amazon.S3.Model;
using CoarUtils.commands.logging;

namespace CoarUtils.commands.aws.s3 {
  public class Exists {
    public static async Task<bool> Execute(
      string awsAccessKey,
      string awsSecretKey,
      Amazon.RegionEndpoint re,
      string key,
      string bucketName,
      CancellationToken cancellationToken
    ) {
      try {
        using (var amazonS3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, re)) {
          var listObjectsRequest = new ListObjectsRequest {
            BucketName = bucketName,
            Prefix = key,
            MaxKeys = 1
          };
          var listObjectResponse = await amazonS3Client.ListObjectsAsync(listObjectsRequest, cancellationToken);
          return listObjectResponse.S3Objects.Any();
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        throw;
      }
    }

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
          var getObjectMetadataResponse = s3c.GetObjectMetadataAsync(request: new Amazon.S3.Model.GetObjectMetadataRequest {
            BucketName = bucketName,
            Key = key
          }).Result;
          url = Constants.S3_BASE + bucketName + "/" + key;
          return getObjectMetadataResponse.ContentLength != 0;
          //return true;
        }
      } catch (Amazon.S3.AmazonS3Exception ex) {
        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
          return false;
        }

        //status wasn't not found, so throw the exception
        throw;
      } catch (Exception ex) {
        if (ex.Message.Contains("Error making request with Error Code NotFound and Http Status Code NotFound")) {
          return false;
        }
        LogIt.E(ex);
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
          var getObjectMetadataResponse = s3c.GetObjectMetadataAsync(request: new Amazon.S3.Model.GetObjectMetadataRequest {
            BucketName = bucketName,
            Key = key
          }).Result;
          contentLength = getObjectMetadataResponse.ContentLength;
          return getObjectMetadataResponse.ContentLength != 0;
        }
      } catch (Amazon.S3.AmazonS3Exception ex) {
        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
          contentLength = 0;
          return false;
        }

        //?
        throw;
      } catch (Exception ex) {
        if (ex.Message.Contains("Error making request with Error Code NotFound and Http Status Code NotFound")) {
          contentLength = 0;
          return false;
        }
        LogIt.E(ex);
        throw;
      }
    }
  }
}

