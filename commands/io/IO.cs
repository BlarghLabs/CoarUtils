using System.IO;

namespace CoarUtils.Utils.IO {
  public class FileAccess {
    public static bool CanRead(string path) {
      try {
        // In .NET Core, the simplest way to check read access is to try opening the directory
        var di = new DirectoryInfo(path);
        di.GetFiles();
        return true;
      } catch {
        return false;
      }
    }
  }
}
