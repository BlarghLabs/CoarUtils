﻿using Amazon.Lambda.Core;
using CoarUtils.commands.reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Diagnostics;
using System.Net;
namespace CoarUtils.commands.logging {
  public class LogIt {
    //this was causing innocious double console log (but only once to file)
    public static bool doNotlogToLambda { get; set; }
    private static readonly NLog.Logger nlogger = NLog.LogManager.GetCurrentClassLogger();
    public enum severity {
      info,
      error,
      warning,
      debug,
      success,
    };
    public static LogLevel GetNLoggerLogLevel(severity s) {
      switch (s) {
        case severity.debug:
          return LogLevel.Debug;
        case severity.info:
          return LogLevel.Info;
        case severity.error:
          return LogLevel.Error;
        case severity.warning:
          return LogLevel.Warn;
        case severity.success:
          return LogLevel.Info;
        default:
          //why required?
          return LogLevel.Info;
      }
    }
    public static void Execute(
      severity s,
      object o,
      string instanceId = null,
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
        var instance = string.IsNullOrWhiteSpace(instanceId) ? "" : $"|{instanceId}";
        var log = $"{dts}{space}|{ss}|{@class}|{method}{instance}|{msg}";
        if (s == severity.error) {
          Console.ForegroundColor = ConsoleColor.Red;
        } else if (s == severity.success) {
          Console.ForegroundColor = ConsoleColor.Green;
        } else if (s == severity.warning) {
          Console.ForegroundColor = ConsoleColor.Yellow;
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
        //TODO: check exists
        LambdaLogger.Log(log);
        //if (_log != null) {
        //  _log.Log(logLevel: GetLogLevel(s), log);
        //}
        nlogger.Log(level: GetNLoggerLogLevel(s), message: log);
      } catch (Exception ex) {
        E("error in LogIt|" + ex.Message);
      }
    }
    public static void Execute(
      severity s,
      string message
    ) {
      try {
        var log = $"{s.ToString().ToUpper()}|{WhereAmI.Execute(stepUp: 3)}|{ message}";
        //TODO: check exists
        LambdaLogger.Log(log);
        //if (_log != null) {
        //  _log.Log(logLevel: GetLogLevel(s), log);
        //}
        nlogger.Log(level: GetNLoggerLogLevel(s), message: log);
      } catch (Exception ex) {
        LogIt.E("error in LogIt|" + ex.Message);
      }
    }
    public static void E(object o, string instanceId = null) {
      try {
        o = o ?? "";
        var t = o.GetType();
        if (!t.Equals(typeof(Exception)) & !typeof(Exception).IsAssignableFrom(t)) {
          Execute(o: o, s: severity.error);
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

          Execute(o: json, s: severity.error, instanceId: instanceId);
        }
      } catch {
        Console.Error.WriteLine("I messed up, this all should be safe from exception");
      }
      Execute(s: severity.error, o: o);
    }
    public static void D(object o = null, string instanceId = null) {
      Execute(s: severity.debug, o: o, instanceId: instanceId);
    }
    public static void I(object o= null, string instanceId = null) {
      Execute(s: severity.info, o: o, instanceId: instanceId);
    }
    public static void W(object o = null, string instanceId = null) {
      Execute(s: severity.warning, o: o, instanceId: instanceId);
    }
    public static void S(object o = null, string instanceId = null) {
      Execute(s: severity.success, o: o, instanceId: instanceId);
    }
  }
}