using Amazon;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancing.Model;
using CoarUtils.commands.logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.aws.elb
{
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

    public static void Execute(
      Request request,
      out HttpStatusCode hsc,
      out Response response,
      out string status,
      CancellationToken cancellationToken,
      HttpContext hc = null
    ) {
      response = new Response { };
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var aelbc = new AmazonElasticLoadBalancingClient(
          awsAccessKeyId: request.awsAccessKey,
          awsSecretAccessKey: request.awsSecretKey,
          region: request.re
        )) {
          var dlbr = aelbc.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest {
             LoadBalancerNames = new List<string> { 
               request.loadBalancerName
             },
          }, cancellationToken: cancellationToken).Result;
          response.total = dlbr.LoadBalancerDescriptions[0].Instances.Count;
          //TODO: get health and unhelathy

          hsc = dlbr.HttpStatusCode;
          return;
        }
        //hsc = HttpStatusCode.OK;
        //status = "";
        //return;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = "unexecpected error";
        return;
      } finally {
        //DO NOT LOG KEYS
        request.awsAccessKey = "DO_NOT_LOG";
        request.awsSecretKey = "DO_NOT_LOG";

        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            request,
            response,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}