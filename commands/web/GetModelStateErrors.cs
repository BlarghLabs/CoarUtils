using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace CoarUtils.commands.web {
  public class GetModelStateErrors {
    public static List<string> Execute(ModelStateDictionary modelState) {
      return modelState.Values
        .SelectMany(state => state.Errors)
        .Select(error => error.ErrorMessage)
        .ToList();
    }
  }
}
