﻿using CoarUtils.commands.logging; using CoarUtils.models.commands; using CoarUtils.models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

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
