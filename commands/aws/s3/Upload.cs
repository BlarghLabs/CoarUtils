using Amazon.S3;
using Amazon.S3.Transfer;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Web;

namespace CoarUtils.commands.aws.s3 {
  public class Upload {
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
          url = Constants.S3_BASE + bucketName + "/" + HttpUtility.UrlEncode(key);
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
            url,
            //ipAddress = GetPublicIpAddress.Execute(),
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
          url = Constants.S3_BASE + bucketName + "/" + HttpUtility.UrlEncode(key);
          hsc = HttpStatusCode.OK;
          return;
        }
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
            url,
            //ipAddress = GetPublicIpAddress.Execute(),
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
        url = Constants.S3_BASE + bucketName + "/" + HttpUtility.UrlEncode(key);
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
            url,
            //ipAddress = GetPublicIpAddress.Execute(),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}


