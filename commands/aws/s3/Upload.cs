using Amazon.S3;
using Amazon.S3.Transfer;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System.Net;
using System.Web;

namespace CoarUtils.commands.aws.s3 {
  public class Upload {
    #region models
    public class Response : models.commands.ResponseStatusModel {
      public string url { get; set; }
    }
    #endregion

    public static async Task<Response> Execute(
      string awsAccessKey,
      string awsSecretKey,
      Amazon.RegionEndpoint re,
      string bucketName,
      byte[] ba,
      string key,
      S3CannedACL acl,
      CancellationToken cancellationToken,
      string contentType = null
    ) {
      var response = new Response { };

      try {
        using (var ms = new MemoryStream(ba)) {
          var uploadMultipartRequest = new TransferUtilityUploadRequest {
            BucketName = bucketName,
            Key = key,
            CannedACL = acl,
            InputStream = ms,
            //PartSize = 123?
          };
          if (!string.IsNullOrWhiteSpace(contentType)) {
            uploadMultipartRequest.ContentType = contentType;
          }
          using (var tu = new TransferUtility(awsAccessKey, awsSecretKey, re)) {
            await tu.UploadAsync(uploadMultipartRequest, cancellationToken);
          }
          //why encoding?
          response.url = HttpUtility.UrlDecode(Constants.Aws.S3_BASE + bucketName + "/" + HttpUtility.UrlEncode(key));
          response.httpStatusCode = HttpStatusCode.OK;
          return response;
        }
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }

        LogIt.E(ex);
        return response = new Response { httpStatusCode = HttpStatusCode.InternalServerError, status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS };
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            response.url,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey,
      Amazon.RegionEndpoint re,
      string bucketName,
      string filePath,
      string key,
      S3CannedACL acl,
      out string url,
      CancellationToken cancellationToken,
      string contentType = null
    ) {
      url = "";
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var tu = new TransferUtility(awsAccessKey, awsSecretKey, re)) {
          var tuur = new TransferUtilityUploadRequest {
            FilePath = filePath,
            BucketName = bucketName,
            Key = key,
            CannedACL = acl
          };
          if (!string.IsNullOrWhiteSpace(contentType)) {
            tuur.ContentType = contentType;
          }
          tu.Upload(tuur);
          //why encoding?
          url = HttpUtility.UrlDecode(Constants.Aws.S3_BASE + bucketName + "/" + HttpUtility.UrlEncode(key));
        }
        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS;
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            url,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out string url,
      string awsAccessKey,
      string awsSecretKey,
      Amazon.RegionEndpoint re,
      string bucketName,
      byte[] ba,
      string key,
      S3CannedACL acl,
      CancellationToken cancellationToken,
      string contentType = null
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      url = "";
      try {
        using (var ms = new MemoryStream(ba)) {
          var uploadMultipartRequest = new TransferUtilityUploadRequest {
            BucketName = bucketName,
            Key = key,
            CannedACL = acl,
            InputStream = ms,
            //PartSize = 123?
          };
          if (!string.IsNullOrWhiteSpace(contentType)) {
            uploadMultipartRequest.ContentType = contentType;
          }
          using (var tu = new TransferUtility(awsAccessKey, awsSecretKey, re)) {
            tu.Upload(uploadMultipartRequest);
          }
          //why encoding?
          url = HttpUtility.UrlDecode(Constants.Aws.S3_BASE + bucketName + "/" + HttpUtility.UrlEncode(key));
          hsc = HttpStatusCode.OK;
          return;
        }
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS;
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            url,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }

    //TODO: do i really want all these to be public read?! are we locking down elsewhre w/ cors or bucket policy?
    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey,
      Amazon.RegionEndpoint re,
      string bucketName,
      MemoryStream ms,
      string key,
      S3CannedACL acl,
      out string url,
      CancellationToken cancellationToken,
      string contentType = null
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      url = "";
      try {
        var uploadMultipartRequest = new TransferUtilityUploadRequest {
          BucketName = bucketName,
          Key = key,
          CannedACL = acl,
          InputStream = ms,
        };
        if (!string.IsNullOrWhiteSpace(contentType)) {
          uploadMultipartRequest.ContentType = contentType;
        }

        using (var tu = new TransferUtility(awsAccessKey, awsSecretKey, re)) {
          tu.Upload(uploadMultipartRequest);
        }
        //why encoding?
        url = HttpUtility.UrlDecode(Constants.Aws.S3_BASE + bucketName + "/" + HttpUtility.UrlEncode(key));
        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS;
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            url,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}


