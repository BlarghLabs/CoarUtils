using System.ComponentModel.DataAnnotations;

namespace CoarUtils.models.commands {
  public class UpdateRequestModel {
    [Required]
    public long requestTimestamp { get; set; }
  }
}




