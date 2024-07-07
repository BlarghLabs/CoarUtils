using System.Net;

namespace CoarUtils.models.sqs {
  public class SqsMessageModel {
    //public string MessageGroupId { get; set; }
    public string receiptHandle { get; set; }
    public string queueUrl { get; set; }

    //public string jsonRequest { get; set; }
  }
}
