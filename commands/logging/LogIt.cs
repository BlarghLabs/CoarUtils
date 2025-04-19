using System.Diagnostics;
using System.Net;
using Amazon.Lambda.Core;
using CoarUtils.commands.reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace CoarUtils.commands.logging {
  public class LogIt {
    #region enums
    public enum Severity {
      info,
      error,
      warning,
      debug,
      success,
    };
    #endregion

    //this was causing innocious double console log (but only once to file)
    public static bool doNotlogToLambda { get; set; } = false; // = true;
    private static readonly NLog.Logger nlogger = NLog.LogManager.GetCurrentClassLogger();
    public const bool DEFAULT_BEEP_BEHAVIOR = false;

    public static NLog.LogLevel GetNLoggerLogLevel(Severity s) {
      switch (s) {
        case Severity.debug:
          return NLog.LogLevel.Debug;
        case Severity.info:
          return NLog.LogLevel.Info;
        case Severity.error:
          return NLog.LogLevel.Error;
        case Severity.warning:
          return NLog.LogLevel.Warn;
        case Severity.success:
          return NLog.LogLevel.Info;
        default:
          //why required?
          return NLog.LogLevel.Info;
      }
    }
    public static void Execute(
      Severity s,
      object o,
      CancellationToken cancellationToken,
      string instanceId = null,
      bool removeNewlinesFromMessage = true,
      bool beepOnWarning = DEFAULT_BEEP_BEHAVIOR,
      bool beepOnError = DEFAULT_BEEP_BEHAVIOR
    ) {
      try {
        //dont check forever
        var methodInfo = new StackFrame(1).GetMethod();
        if (methodInfo?.DeclaringType?.Name == typeof(LogIt).Name) {
          methodInfo = new StackFrame(2).GetMethod();
        }
        if (methodInfo?.DeclaringType?.Name == typeof(LogIt).Name) {
          methodInfo = new StackFrame(3).GetMethod();
        }
        var className = "";
        if (methodInfo.DeclaringType != null) {
          if (methodInfo.DeclaringType.Name.StartsWith("<") || methodInfo.DeclaringType.Name.EndsWith(">")) {
            className = methodInfo.DeclaringType.ReflectedType.Name;
          } else {
            className = methodInfo.DeclaringType.Name;
          }
        }
        var method = "";
        if (methodInfo.DeclaringType != null) {
          if (methodInfo.Name.Equals("MoveNext")) {
            if (
              methodInfo.DeclaringType.Name.Contains("<")
              &&
              methodInfo.DeclaringType.Name.Contains(">")
              &&
              (methodInfo.DeclaringType.Name.IndexOf("<") < methodInfo.DeclaringType.Name.IndexOf(">"))
            ) {
              int pFrom = methodInfo.DeclaringType.Name.IndexOf("<") + "<".Length;
              int pTo = methodInfo.DeclaringType.Name.LastIndexOf(">");
              method = methodInfo.DeclaringType.Name.Substring(pFrom, pTo - pFrom);
            } else {
              method = methodInfo.DeclaringType.Name;
            }
          } else {
            method = methodInfo.Name;
          }
        }
        var nameSpace = "";
        if (methodInfo.DeclaringType != null) {
          nameSpace = methodInfo.DeclaringType.Namespace;
        }

        //for format consistency
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/bb926074-d593-4e0b-8754-7026acc607ec/datetime-tostring-colon-replaced-with-period?forum=csharpgeneral
        var dt = DateTime.UtcNow;
        var dts = dt.ToString("yyyy-MM-dd HH") + ":" + dt.ToString("mm") + ":" + dt.ToString("ss") + "." + dt.ToString("fff");
        var ss = "[" + s.ToString().ToUpper() + "]";
        o = o ?? "";
        var msg = o.ToString(); //json convert instead?
        msg = !removeNewlinesFromMessage
          ? msg
          : msg.Replace("\r\n", " ").Replace("\n", " ")
        ; //currently just a string
          //var whereIAm = WhereAmI.Execute(stepUp: 3);
          //this is for cloud watch logs which requires space after timestamp: http://docs.aws.amazon.com/AWSEC2/latest/WindowsGuide/send_logs_to_cwl.html
        var space = " ";
        var instance = string.IsNullOrWhiteSpace(instanceId) ? "" : $"|{instanceId}";
        var log = $"{dts}{space}|{ss}|{nameSpace}|{className}|{method}{instance}|{msg}";
        if (s == Severity.error) {
          Console.ForegroundColor = ConsoleColor.Red;
          if (beepOnError) {
            Console.Beep();// 38, 1000);
          }
        } else if (s == Severity.success) {
          Console.ForegroundColor = ConsoleColor.Green;
        } else if (s == Severity.warning) {
          Console.ForegroundColor = ConsoleColor.Yellow;
          if (beepOnWarning) {
            Console.Beep(); // 3800, 500);          
          }
        } else {
          Console.ForegroundColor = ConsoleColor.White;
        }
        //TODO: check exists
        if (!doNotlogToLambda) {
          LambdaLogger.Log(log);
        }
        //if (_log != null) {
        //  _log.Log(logLevel: GetLogLevel(s), log);
        //}
        nlogger.Log(level: GetNLoggerLogLevel(s), message: log);
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return;
        }
        E("error in LogIt|" + ex.Message, cancellationToken);
      }
    }
    public static void Execute(
      HttpStatusCode hsc,
      object o,
      CancellationToken cancellationToken,
      bool removeNewlinesFromMessage = true
    ) {
      try {
        var methodInfo = new StackFrame(1).GetMethod();
        if (methodInfo.DeclaringType.Name == typeof(LogIt).Name) {
          methodInfo = new StackFrame(2).GetMethod();
        }
        if (methodInfo.DeclaringType.Name == typeof(LogIt).Name) {
          methodInfo = new StackFrame(3).GetMethod();
        }
        var className = "";
        if (methodInfo.DeclaringType != null) {
          if (methodInfo.DeclaringType.Name.StartsWith("<") || methodInfo.DeclaringType.Name.EndsWith(">")) {
            className = methodInfo.DeclaringType.ReflectedType.Name;
          } else {
            className = methodInfo.DeclaringType.Name;
          }
        }
        var method = "";
        if (methodInfo.DeclaringType != null) {
          if (methodInfo.Name.Equals("MoveNext")) {
            if (
              methodInfo.DeclaringType.Name.Contains("<")
              &&
              methodInfo.DeclaringType.Name.Contains(">")
              &&
              (methodInfo.DeclaringType.Name.IndexOf("<") < methodInfo.DeclaringType.Name.IndexOf(">"))
            ) {
              int pFrom = methodInfo.DeclaringType.Name.IndexOf("<") + "<".Length;
              int pTo = methodInfo.DeclaringType.Name.LastIndexOf(">");
              method = methodInfo.DeclaringType.Name.Substring(pFrom, pTo - pFrom);
            } else {
              method = methodInfo.DeclaringType.Name;
            }
          } else {
            method = methodInfo.Name;
          }
        }
        var nameSpace = "";
        if (methodInfo.DeclaringType != null) {
          nameSpace = methodInfo.DeclaringType.Namespace;
        }
        //for format consistency
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/bb926074-d593-4e0b-8754-7026acc607ec/datetime-tostring-colon-replaced-with-period?forum=csharpgeneral
        var dt = DateTime.UtcNow;
        var dts = dt.ToString("yyyy-MM-dd HH") + ":" + dt.ToString("mm") + ":" + dt.ToString("ss") + "." + dt.ToString("fff");
        var s = Severity.info;
        switch (hsc) {
          case HttpStatusCode.OK:
            s = Severity.info;
            break;
          default:
            s = Severity.error;
            break;
        }
        var ss = "[" + s.ToString().ToUpper() + "]";
        o = o ?? "";
        var msg = o.ToString(); //json convert instead?
        msg = !removeNewlinesFromMessage
          ? msg
          : msg.Replace("\r\n", " ").Replace("\n", " ")
        ; //currently just a string
          //var whereIAm = WhereAmI.Execute(stepUp: 3);
          //this is for cloud watch logs which requires space after timestamp: http://docs.aws.amazon.com/AWSEC2/latest/WindowsGuide/send_logs_to_cwl.html
        var space = " ";
        var log = $"{dts}{space}|{ss}|{nameSpace}|{className}|{method}|{msg}";
        //TODO: check exists
        LambdaLogger.Log(log);
        //if (_log != null) {
        //  _log.Log(logLevel: GetLogLevel(s), log);
        //}
        nlogger.Log(level: GetNLoggerLogLevel(s), message: log);
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return;
        }
        E("error in LogIt|" + ex.Message, cancellationToken);
      }
    }
    public static void Execute(
      Severity s,
      string message,
      CancellationToken cancellationToken
    ) {
      try {
        var log = $"{s.ToString().ToUpper()}|{WhereAmI.Execute(stepUp: 3)}|{message}";
        //TODO: check exists
        LambdaLogger.Log(log);
        //if (_log != null) {
        //  _log.Log(logLevel: GetLogLevel(s), log);
        //}
        nlogger.Log(level: GetNLoggerLogLevel(s), message: log);
      } catch (Exception ex) {
        LogIt.E("error in LogIt|" + ex.Message, cancellationToken);
      }
    }
    public static void E(object o, CancellationToken cancellationToken, string instanceId = null) {
      try {
        o = o ?? "";
        var t = o.GetType();
        if (!t.Equals(typeof(Exception)) & !typeof(Exception).IsAssignableFrom(t)) {
          Execute(o: o, s: Severity.error, cancellationToken: cancellationToken);
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
            (
              ex.Message.Contains("See the inner exception for details.")
              ||
              ex.Message.Contains("See Status or InnerException for more information.")
            )
          ) {
            if (!ex.InnerException.Message.Contains("See the inner exception for details.")) {
              error.innerExceptionMessage = ex.InnerException.Message;
            } else if (
              (ex.InnerException.InnerException != null)
              &&
              !string.IsNullOrEmpty(ex.InnerException.InnerException.Message)
              &&
              (
                !ex.InnerException.InnerException.Message.Contains("See the inner exception for details")
                &&
                !ex.InnerException.InnerException.Message.Contains("See Status or InnerException for more information")
              )
            ) {
              error.innerExceptionMessage = ex.InnerException.InnerException.Message;
            }
          }
          string json = JsonConvert.SerializeObject(error, Formatting.Indented);

          Execute(o: json, s: Severity.error, instanceId: instanceId, cancellationToken: cancellationToken);
          return; //?
        }
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return;
        }
        Console.Error.WriteLine("I messed up, this all should be safe from exception");
        Console.Error.WriteLine(ex.Message);
      }
      //is this supposed to be here twice?
      Execute(s: Severity.error, o: o, cancellationToken: cancellationToken);
    }
    //public static void D(CancellationToken cancellationToken, object o = null, string instanceId = null) {
    public static void D(object o, CancellationToken cancellationToken, string instanceId = null) {
      Execute(s: Severity.debug, o: o, instanceId: instanceId, cancellationToken: cancellationToken);
    }
    public static void I(object o, CancellationToken cancellationToken, string instanceId = null) {
      Execute(s: Severity.info, o: o, instanceId: instanceId, cancellationToken: cancellationToken);
    }
    public static void W(object o, CancellationToken cancellationToken, string instanceId = null, bool beep = DEFAULT_BEEP_BEHAVIOR) {
      Execute(s: Severity.warning, o: o, instanceId: instanceId, beepOnWarning: beep, cancellationToken: cancellationToken);
    }
    public static void S(object o, CancellationToken cancellationToken, string instanceId = null) {
      Execute(s: Severity.success, o: o, instanceId: instanceId, cancellationToken: cancellationToken);
    }
  }
}