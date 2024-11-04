using CoarUtils.commands.logging; using CoarUtils.models.commands; using CoarUtils.models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CoarUtils.commands.currency {
  //https://stackoverflow.com/questions/12373800/3-digit-currency-code-to-currency-symbol
  public static class GetCurrencySymbol {
    private static IDictionary<string, string> map;
    static GetCurrencySymbol () {
      try {
        var loci = CultureInfo
          .GetCultures(CultureTypes.AllCultures)
          .Where(c => !c.IsNeutralCulture)
          .ToList()
        ;
        map = loci
            .Select(culture => {
              try {
                return new RegionInfo(culture.LCID);
              } catch {
                return null;
              }
            })
            .Where(ri => ri != null)
            .GroupBy(ri => ri.ISOCurrencySymbol)
            .ToDictionary(x => x.Key, x => x.First().CurrencySymbol);
      } catch (Exception ex) {
        LogIt.E(ex);
        throw ex;
      }
    }
    public static bool TryGet(
      string ISOCurrencySymbol,
      out string symbol
    ) {
      if (!string.IsNullOrWhiteSpace(ISOCurrencySymbol)) {
        ISOCurrencySymbol = ISOCurrencySymbol.ToUpper();
      }
      return map.TryGetValue(ISOCurrencySymbol, out symbol);
    }

    public static bool Execute(
      string ISOCurrencySymbol,
      out string symbol
    ) {
      if (!string.IsNullOrWhiteSpace(ISOCurrencySymbol)){
        ISOCurrencySymbol = ISOCurrencySymbol.ToUpper();
      }
      var success = map.TryGetValue(ISOCurrencySymbol, out symbol);
      return success;

    }
  }
}



