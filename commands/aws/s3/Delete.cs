﻿using Amazon;
using Amazon.S3;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.aws.s3 {
  public class Delete {
    public class Request {
      public string bucketName { get; set; }
      public string key { get; set; }
      public RegionEndpoint re { get; set; }
    }


    public static void Execute(
      Request request,
      out HttpStatusCode hsc,
      out string status,
      string awsAccessKey,
      string awsSecretKey,
      CancellationToken cancellationToken
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var s3c = new AmazonS3Client(awsAccessKey, awsSecretKey, request.re)) {
          var deleteObjectRequest = new Amazon.S3.Model.DeleteObjectRequest {
            BucketName = request.bucketName,
            Key = request.key,
          };
          var dor = s3c.DeleteObjectAsync(deleteObjectRequest).Result;
          hsc = dor.HttpStatusCode == System.Net.HttpStatusCode.NoContent
            ? HttpStatusCode.OK
            : HttpStatusCode.BadRequest
          ;
        }
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
            request,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }

  }
}







