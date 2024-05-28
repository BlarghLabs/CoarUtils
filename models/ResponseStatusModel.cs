using System.Net;

namespace CoarUtils.models {
  public class ResponseStatusModel {
    public HttpStatusCode httpStatusCode { get; set; } = HttpStatusCode.BadRequest;
    public string status { get; set; }
  }
}
