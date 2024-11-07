using System.Diagnostics;

namespace CoarUtils.commands.reflection {
  public class WhereAmI {
    public static string Execute(int stepUp = 1) {
      var mb = new StackFrame(stepUp).GetMethod();
      var classAndMethod = mb.DeclaringType.Name + "|" + mb.Name;
      return classAndMethod;
    }
  }
}
