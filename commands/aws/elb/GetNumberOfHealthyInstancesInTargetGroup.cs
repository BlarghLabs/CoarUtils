using System.Net;
using Amazon;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancing.Model;
using CoarUtils.commands.logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CoarUtils.commands.aws.elb {
  public class GetNumberOfHealthyInstancesInTargetGroup {
    #region models
    public class Request {
      public string awsAccessKey { get; set; }
      public string awsSecretKey { get; set; }
      public RegionEndpoint re { get; set; }
      public string loadBalancerName { get; set; }
    }

    public class Response : models.commands.ResponseStatusModel {
      public int healthy { get; set; }
      public int unhealthy { get; set; }
      public int total { get; set; }
    }

    #endregion

    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken,
      HttpContext hc = null
    ) {
      var response = new Response { };
      try {
        using (var aelbc = new AmazonElasticLoadBalancingClient(
          awsAccessKeyId: request.awsAccessKey,
          awsSecretAccessKey: request.awsSecretKey,
          region: request.re
        )) {
          var dlbr = await aelbc.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest {
            LoadBalancerNames = new List<string> {
               request.loadBalancerName
             },
          }, cancellationToken: cancellationToken).ConfigureAwait(false);
          response.total = dlbr.LoadBalancerDescriptions[0].Instances.Count;
          //TODO: get health and unhelathy

          response.httpStatusCode = dlbr.HttpStatusCode;
          return response;
        }
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.E(ex, cancellationToken);
        return response = new Response { status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS, httpStatusCode = HttpStatusCode.InternalServerError };
      } finally {
        //DO NOT LOG KEYS
        request.awsAccessKey = "DO_NOT_LOG";
        request.awsSecretKey = "DO_NOT_LOG";

        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            request,
            response,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented), cancellationToken);
      }
    }
  }
}