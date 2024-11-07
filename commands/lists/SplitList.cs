namespace CoarUtils.commands.lists {
  public class SplitList {
    public static IEnumerable<List<T>> Execute<T>(List<T> locations, int nSize = 30) {
      for (int i = 0; i < locations.Count; i += nSize) {
        yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
      }
    }
  }
}
