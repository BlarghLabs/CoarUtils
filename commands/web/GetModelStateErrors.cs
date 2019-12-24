using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CoarUtils.commands.web {
  public class GetModelStateErrors {
    public static List<string> Execute(
      ModelStateDictionary modelState
    ) {

      var query = from state in modelState.Values
                  from error in state.Errors
                  select error.ErrorMessage;

      var errorList = query.ToList();
      return errorList;
    }
  }
}
