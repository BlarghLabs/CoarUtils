using System.Net;
using CoarUtils.models.commands;

namespace CoarUtils.models.commands {
  public class TableResponseStatusModel : ResponseStatusModel {
    public int pageSize { get; set; }
    public int pageNum { get; set; }
    public int skip => (pageNum - 1) * pageSize;
    public int firstRowNumber => skip + 1;
    public int lastRowNumber => Math.Min(pageSize * pageNum, totalRecordCount);
    public bool paginationPreviousIsEnabled => firstRowNumber > 1;
    public bool paginationNextIsEnabled => Math.Min((pageSize * pageNum), totalRecordCount) < totalRecordCount;
    public int totalRecordCount { get; set; }
  }
}