using System;

namespace CoarUtils.commands.exceptions {
  public static class GetBaseException {
    //https://stackoverflow.com/questions/3876456/find-the-inner-most-exception-without-using-a-while-loop
    public static Exception Execute(Exception ex) {
      try {
        var y = ex;
        while (y.InnerException != null) y = y.InnerException;
        return y;
      } catch (Exception ex1) {
        Console.WriteLine(ex1.Message);
        return ex;
      }
    }
  }
}
