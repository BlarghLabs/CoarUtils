using CoarUtils.commands.logging;
using Newtonsoft.Json;

namespace CoarUtils.commands.debugging {
  public class GetJsonString {
    public static string Execute(object o) {
      try {
        return JsonConvert.SerializeObject(o);
      } catch (Exception ex) {
        LogIt.E(ex);
      }
      return "";
    }
  }
}
