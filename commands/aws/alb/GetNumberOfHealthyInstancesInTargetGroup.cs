using Amazon;
using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using CoarUtils.commands.logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.aws.alb {
  public class GetNumberOfHealthyInstancesInTargetGroup {
    #region models
    public class Request {
      public string awsAccessKey { get; set; }
      public string awsSecretKey { get; set; }
      public RegionEndpoint re { get; set; }
      public string targetGroupArn { get; set; }
      public string loadBalancerArn { get; set; }
    }

    public class Response {
      public bool isActive { get; set; }
      public int healthy { get; set; }
      public int unhealthy { get; set; }
      public int total { get; set; }
    }

    #endregion

    public static void Execute(
      Request m,
      out HttpStatusCode hsc,
      out Response r,
      out string status,
      HttpContext hc = null,
      CancellationToken? ct = null
    ) {
      r = new Response { };
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var aelbc = new AmazonElasticLoadBalancingV2Client(
          awsAccessKeyId: m.awsAccessKey,
          awsSecretAccessKey: m.awsSecretKey,
          region: m.re
        )) {
          var describeLoadBalancersResponse = aelbc.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest {
             LoadBalancerArns = new List<string> { m.loadBalancerArn },
          }, cancellationToken: ct.HasValue ? ct.Value : CancellationToken.None).Result;
          r.isActive = describeLoadBalancersResponse.LoadBalancers.First().State.Code == LoadBalancerStateEnum.Active;

          //var describeTargetGroupsResponse = aelbc.DescribeTargetGroupsAsync(new DescribeTargetGroupsRequest{
          //   TargetGroupArns =  new List<string> { m.targetGroupArn },
          //}, cancellationToken: ct.HasValue ? ct.Value : CancellationToken.None).Result;

          var describeTargetHealthResponse = aelbc.DescribeTargetHealthAsync(new DescribeTargetHealthRequest{
             TargetGroupArn = m.targetGroupArn,
          }, cancellationToken: ct.HasValue ? ct.Value : CancellationToken.None).Result;

          r.healthy = describeTargetHealthResponse.TargetHealthDescriptions
            .Count(x => x.TargetHealth.State == TargetHealthStateEnum.Healthy)
          ;
          r.unhealthy = describeTargetHealthResponse.TargetHealthDescriptions
            .Count(x => x.TargetHealth.State != TargetHealthStateEnum.Healthy)
          ;
          r.total = describeTargetHealthResponse.TargetHealthDescriptions
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
        if (ct.HasValue && ct.Value.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = "task cancelled";
          return;
        }

        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = "unexecpected error";
        return;
      } finally {
        //DO NOT LOG KEYS
        m.awsAccessKey = "DO_NOT_LOG";
        m.awsSecretKey = "DO_NOT_LOG";

        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            m,
            r,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}