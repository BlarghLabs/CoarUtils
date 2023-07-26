using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System.Globalization;

namespace CoarUtils.commands.gis {
  //https://stackoverflow.com/questions/4884692/converting-country-codes-in-net
  public static class Iso3CountryCodeToIso2CountryCode {
    public static string Execute(string iso3CountryCode) {
      string iso2CountryCode = null;
      try {
        if (string.IsNullOrEmpty(iso3CountryCode)) {
          return null;
        }
        if (iso3CountryCode.Length != 3) {
          throw new ArgumentException("must be three letters");
        }

        iso3CountryCode = iso3CountryCode.ToUpper();

        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        foreach (CultureInfo culture in cultures) {
          RegionInfo region = new RegionInfo(culture.LCID);
          if (region.ThreeLetterISORegionName.ToUpper() == iso3CountryCode) {
            iso2CountryCode = region.TwoLetterISORegionName;
            return iso2CountryCode;
          }
        }
        return iso2CountryCode;
      } catch (Exception ex) {
        LogIt.E(ex);
        return iso2CountryCode;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            iso3CountryCode,
            iso2CountryCode
          }, Formatting.Indented));
      }

    }
  }

}
