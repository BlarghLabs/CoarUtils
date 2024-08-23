using Amazon;
using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using CoarUtils.commands.logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.aws.alb
{
    public class GetNumberOfHealthyInstancesInTargetGroup {
    #region models
    public class Request {
      public string awsAccessKey { get; set; }
      public string awsSecretKey { get; set; }
      public RegionEndpoint re { get; set; }
      public string targetGroupArn { get; set; }
      public string loadBalancerArn { get; set; }
    }

    public class Response : models.commands.ResponseStatusModel {
      public bool isActive { get; set; }
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
        using (var aelbc = new AmazonElasticLoadBalancingV2Client(
          awsAccessKeyId: request.awsAccessKey,
          awsSecretAccessKey: request.awsSecretKey,
          region: request.re
        )) {
          var describeLoadBalancersResponse = aelbc.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest {
             LoadBalancerArns = new List<string> { request.loadBalancerArn },
          }, cancellationToken: cancellationToken).Result;
          response.isActive = describeLoadBalancersResponse.LoadBalancers.First().State.Code == LoadBalancerStateEnum.Active;

          //var describeTargetGroupsResponse = aelbc.DescribeTargetGroupsAsync(new DescribeTargetGroupsRequest{
          //   TargetGroupArns =  new List<string> { request.targetGroupArn },
          //}, cancellationToken: cancellationToken).Result;

          var describeTargetHealthResponse = aelbc.DescribeTargetHealthAsync(new DescribeTargetHealthRequest{
             TargetGroupArn = request.targetGroupArn,
          }, cancellationToken: cancellationToken).Result;

          response.healthy = describeTargetHealthResponse.TargetHealthDescriptions
            .Count(x => x.TargetHealth.State == TargetHealthStateEnum.Healthy)
          ;
          response.unhealthy = describeTargetHealthResponse.TargetHealthDescriptions
            .Count(x => x.TargetHealth.State != TargetHealthStateEnum.Healthy)
          ;
          response.total = describeTargetHealthResponse.TargetHealthDescriptions
            .Count()
          ;
          hsc = describeTargetHealthResponse.HttpStatusCode
            //&& 
            //describeLoadBalancersResponse.HttpStatusCode
          ;
          return;
        }
        //hsc = HttpStatusCode.OK;
        //status = "";
        //return;
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