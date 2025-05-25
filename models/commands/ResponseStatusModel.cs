using System.Net;

namespace CoarUtils.models.commands {
  public class ResponseStatusModel {
    //why is this commented out?
    //public ResponseStatusModel(string status) {
    //  this.status = status;
    //}
    public HttpStatusCode httpStatusCode { get; set; } = HttpStatusCode.BadRequest;
    public string status { get; set; }
  }
}
