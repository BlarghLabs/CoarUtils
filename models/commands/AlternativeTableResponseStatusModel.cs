namespace CoarUtils.models.commands {
  public class AlternativeTableResponseStatusModel : ResponseStatusModel {

    public int totalRecordCount;
    public int? pageSize = 10;
    public int? pageNum = 1;
    public bool loadMore => Math.Min((int)(pageSize * pageNum), totalRecordCount) < totalRecordCount;
    public string loadedRecords => Math.Min((int)(pageSize * pageNum), totalRecordCount) + "/" + totalRecordCount;
  }
}




