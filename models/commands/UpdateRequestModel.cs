using System.ComponentModel.DataAnnotations;
using System.Net;
using CoarUtils.models.commands;

namespace CoarUtils.models.commands {
  public class UpdateRequestModel {
    [Required]
    public long requestTimestamp { get; set; }
  }
}

 


