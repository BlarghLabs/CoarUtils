using System.Globalization;
using CoarUtils.commands.logging;
using Newtonsoft.Json;

namespace CoarUtils.commands.gis {
  //https://stackoverflow.com/questions/4884692/converting-country-codes-in-net
  //ulture ID 4096 (0x1000) is a neutral culture; a region cannot be created from it. (Parameter 'culture')
  //var twoLetterCountryCode = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(ci => new RegionInfo(ci.LCID)).FirstOrDefault(r => r.ThreeLetterISORegionName.Equals(threeLetterCountryCode, StringComparison.InvariantCultureIgnoreCase))                                     ?.TwoLetterISORegionName;
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

        //thorws error on server w/ neutral
        var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => !x.IsNeutralCulture).ToList();
        foreach (var cultureInfo in cultureInfos) {
          RegionInfo region = new RegionInfo(cultureInfo.LCID);
          if (region.ThreeLetterISORegionName.ToUpper() == iso3CountryCode) {
            iso2CountryCode = region.TwoLetterISORegionName;
            return iso2CountryCode;
          }
        }
        return iso2CountryCode;
      } catch (Exception ex) {
        LogIt.I(ex, CancellationToken.None);
        return iso2CountryCode;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            iso3CountryCode,
            iso2CountryCode
          }, Formatting.Indented), CancellationToken.None);
      }

    }
  }

}
