using Amazon.Lambda.Core;
using CoarUtils.commands.reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;

namespace CoarUtils.commands.logging {
  public class LogIt {
    public enum severity {
      info,
      error,
      warning,
      debug,
    };

    public static void Execute(
      severity s,
      object o,
      bool removeNewlinesFromMessage = true
    ) {
      try {
        //dont check forever
        var methodInfo = new StackFrame(1).GetMethod();
        if (methodInfo.DeclaringType.Name == typeof(LogIt).Name) {
          methodInfo = new StackFrame(2).GetMethod();
        }
        if (methodInfo.DeclaringType.Name == typeof(LogIt).Name) {
          methodInfo = new StackFrame(3).GetMethod();
        }

        //for format consistency
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/bb926074-d593-4e0b-8754-7026acc607ec/datetime-tostring-colon-replaced-with-period?forum=csharpgeneral
        var dt = DateTime.UtcNow;
        var dts = dt.ToString("yyyy-MM-dd HH") + ":" + dt.ToString("mm") + ":" + dt.ToString("ss") + "." + dt.ToString("fff");
        var ss = "[" + s.ToString().ToUpper() + "]";
        var @class = (methodInfo.DeclaringType == null)
          ? ""
          : methodInfo.DeclaringType.Name
        ;
        var method = (methodInfo.DeclaringType == null)
          ? ""
          : methodInfo.Name
        ;
        o = o ?? "";
        var msg = o.ToString(); //json convert instead?
        msg = !removeNewlinesFromMessage
          ? msg
          : msg.Replace("\r\n", " ").Replace("\n", " ")
        ; //currently just a string
        //var whereIAm = WhereAmI.Execute(stepUp: 3);

        //this is for cloud watch logs which requires space after timestamp: http://docs.aws.amazon.com/AWSEC2/latest/WindowsGuide/send_logs_to_cwl.html
        var space = " ";
        var log = $"{dts}{space}|{ss}|{@class}|{method}|{msg}";

        if (s == severity.error) {
          Console.ForegroundColor = ConsoleColor.Red;
        } else if (s == severity.warning) {
          Console.ForegroundColor = ConsoleColor.Yellow;
        } else {
          Console.ForegroundColor = ConsoleColor.White;
        }
        LambdaLogger.Log(log);
      } catch (Exception ex) {
        E("error in LogIt|" + ex.Message);
      }
    }
    public static void Execute(
      HttpStatusCode hsc,
      object o,
      bool removeNewlinesFromMessage = true
    ) {
      try {
        //dont check forever
        var methodInfo = new StackFrame(1).GetMethod();
        if (methodInfo.DeclaringType.Name == typeof(LogIt).Name) {
          methodInfo = new StackFrame(2).GetMethod();
        }
        if (methodInfo.DeclaringType.Name == typeof(LogIt).Name) {
          methodInfo = new StackFrame(3).GetMethod();
        }

        //for format consistency
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/bb926074-d593-4e0b-8754-7026acc607ec/datetime-tostring-colon-replaced-with-period?forum=csharpgeneral
        var dt = DateTime.UtcNow;
        var dts = dt.ToString("yyyy-MM-dd HH") + ":" + dt.ToString("mm") + ":" + dt.ToString("ss") + "." + dt.ToString("fff");
        var s = severity.info;
        switch (hsc) {
          case HttpStatusCode.OK:
            s = severity.info;
            break;
          default:
            s = severity.error;
            break;
        }
        var ss = "[" + s.ToString().ToUpper() + "]";
        var @class = (methodInfo.DeclaringType == null)
          ? ""
          : methodInfo.DeclaringType.Name
        ;
        var method = (methodInfo.DeclaringType == null)
          ? ""
          : methodInfo.Name
        ;
        o = o ?? "";
        var msg = o.ToString(); //json convert instead?
        msg = !removeNewlinesFromMessage
          ? msg
          : msg.Replace("\r\n", " ").Replace("\n", " ")
        ; //currently just a string
        //var whereIAm = WhereAmI.Execute(stepUp: 3);

        //this is for cloud watch logs which requires space after timestamp: http://docs.aws.amazon.com/AWSEC2/latest/WindowsGuide/send_logs_to_cwl.html
        var space = " ";
        var log = $"{dts}{space}|{ss}|{@class}|{method}|{msg}";

        LambdaLogger.Log(log);
      } catch (Exception ex) {
        E("error in LogIt|" + ex.Message);
      }
    }


    public static void Execute(
      severity s,
      string message
    ) {
      try {
        LambdaLogger.Log($"{s.ToString().ToUpper()}|{WhereAmI.Execute(stepUp: 3)}|{ message}");
      } catch (Exception ex) {
        LogIt.E("error in LogIt|" + ex.Message);
      }
    }

    public static void E(object o) {
      try {
        o = o ?? "";
        var t = o.GetType();
        if (!t.Equals(typeof(Exception)) & !typeof(Exception).IsAssignableFrom(t)) {
          Execute(o:o, s: severity.error);
        } else {
          var ex = (Exception)o;
          dynamic error = new JObject();
          error.message = ex.Message;
          error.stackTrace = ex.StackTrace;

          if (
            (ex.InnerException != null)
            &&
            !string.IsNullOrEmpty(ex.InnerException.Message)
            &&
            ex.Message.Contains("See the inner exception for details.")
          ) {
            if (!ex.InnerException.Message.Contains("See the inner exception for details.")) {
              error.innerExceptionMessage = ex.InnerException.Message;
            } else if (
              (ex.InnerException.InnerException != null)
              &&
              !string.IsNullOrEmpty(ex.InnerException.InnerException.Message)
              &&
              !ex.InnerException.InnerException.Message.Contains("See the inner exception for details")
            ) {
              error.innerExceptionMessage = ex.InnerException.InnerException.Message;
            }
          }

          string json = JsonConvert.SerializeObject(error, Formatting.Indented);
          Execute(o: json, s: severity.error);
        }
      } catch {
        Console.Error.WriteLine("I messed up, this all should be safe from exception");
      }


      Execute(s: severity.error, o: o);
    }
    public static void D(object o) {
      Execute(s: severity.debug, o: o);
    }
    public static void I(object o) {
      Execute(s: severity.info, o: o);
    }
    public static void W(object o) {
      Execute(s: severity.warning, o: o);
    }

    //public static void E(string message) {
    //  Execute(s: severity.error, message: message);
    //}
    //public static void D(string message) {
    //  Execute(s: severity.debug, message: message);
    //}
    //public static void I(string message) {
    //  Execute(s: severity.info, message: message);
    //}
    //public static void W(string message) {
    //  Execute(s: severity.warning, message: message);
    //}
  }
}
