using Amazon.S3;
using Amazon.S3.Model;
using CoarUtils.commands.logging;
namespace CoarUtils.commands.aws.s3 {
  public class Exists {
    public static async Task<bool> Execute(
      string awsAccessKey,
      string awsSecretKey,
      Amazon.RegionEndpoint regionEndpoint,
      string key,
      string bucketName,
      CancellationToken cancellationToken
    ) {
      try {
        using (var amazonS3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, regionEndpoint)) {
          //is this a more expensive request bc it is list?
          var listObjectsRequest = new ListObjectsRequest {
            BucketName = bucketName,
            Prefix = key,
            MaxKeys = 1
          };
          var listObjectResponse = await amazonS3Client.ListObjectsAsync(listObjectsRequest, cancellationToken).ConfigureAwait(false);
          return listObjectResponse.S3Objects.Any();
        }
      } catch (Exception ex) {
        LogIt.E(ex, cancellationToken);
        throw;
      }
    }
  }
}

