using Amazon.S3;
using Amazon.S3.Model;
using CoarUtils.commands.logging;
using CoarUtils.commands.web;
using CoarUtils.models.commands;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.aws.s3 {
  #region models
  public class Response : ResponseStatusModel {
    public string url { get; set; }
  }
  #endregion

  //TODO: on fail behavior ... what to do?
  public class CreatePresignedUrl {
    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out string url,
      string awsAccessKey,
      string awsSecretKey,
      string bucketName,
      string key,
      Amazon.RegionEndpoint re,
      CancellationToken cancellationToken,
      HttpContext hc = null,
      int numberOfMinutes = 30
    ) {
      url = "";
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        using (var s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, re)) {
          var gpsur = new GetPreSignedUrlRequest {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(numberOfMinutes)
          };
          url = s3Client.GetPreSignedURL(gpsur);
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
        status = "unexecpected error";
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            bucketName,
            key,
            url,
            numberOfMinutes,
            ipAddress = GetPublicIpAddress.Execute(hc),
            executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }

    public static Response Execute(
      string awsAccessKey,
      string awsSecretKey,
      string bucketName,
      string key,
      Amazon.RegionEndpoint re,
      CancellationToken cancellationToken,
      HttpContext hc = null,
      int numberOfMinutes = 30
    ) {
      var response = new Response { };
      try {
        using (var s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, re)) {
          var gpsur = new GetPreSignedUrlRequest {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(numberOfMinutes)
          };
          response.url = s3Client.GetPreSignedURL(gpsur);
        }
        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }

        LogIt.E(ex);
        response.httpStatusCode = HttpStatusCode.InternalServerError;
        response.status = "unexecpected error";
        return response;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            bucketName,
            key,
            response.url,
            numberOfMinutes,
            ipAddress = GetPublicIpAddress.Execute(hc),
            executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }

  }
}


