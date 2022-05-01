using Amazon;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancing.Model;
using CoarUtils.commands.logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.aws.ec2 {
  public class GetNumberOfHealthyInstancesInTargetGroup {
    #region models
    public class request {
      public string awsAccessKey { get; set; }
      public string awsSecretKey { get; set; }
      public RegionEndpoint re { get; set; }
      public string loadBalancerName { get; set; }
    }

    public class response {
      public int healthy { get; set; }
      public int unhealthy { get; set; }
      public int total { get; set; }
    }

    #endregion

    public static void Execute(
      request m,
      out HttpStatusCode hsc,
      out response r,
      out string status,
      HttpContext hc = null,
      CancellationToken? ct = null
    ) {
      r = new response { };
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var aelbc = new AmazonElasticLoadBalancingClient(
          awsAccessKeyId: m.awsAccessKey,
          awsSecretAccessKey: m.awsSecretKey,
          region: Amazon.RegionEndpoint.USEast1
        )) {


          var dlbr = aelbc.DescribeLoadBalancersAsync(new DescribeLoadBalancersRequest {
             LoadBalancerNames = new List<string> { m.loadBalancerName }
          }, cancellationToken: ct.HasValue ? ct.Value : CancellationToken.None).Result;

          r.total = dlbr.LoadBalancerDescriptions[0].Instances.Count;
          //TODO: get health and unhelathy

          hsc = dlbr.HttpStatusCode;
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