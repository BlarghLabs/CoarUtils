﻿using Amazon.S3;
using Amazon.S3.Model;
using CoarUtils.commands.logging;
using CoarUtils.commands.web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;

namespace CoarUtils.commands.aws.s3 {

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
      HttpContext hc = null,
      int numberOfMinutes = 30,
      CancellationToken? ct = null
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
        if (ct.HasValue && ct.Value.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.CANCELLATION_REQUESTED_STATUS;
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

  }
}


