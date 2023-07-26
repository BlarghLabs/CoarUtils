using System.Globalization;

namespace CoarUtils.commands.gis {
  //https://stackoverflow.com/questions/4884692/converting-country-codes-in-net
  public static class Iso3CountryCodeToIso2CountryCode {
    public static string Execute(string iso3CountryCode) {
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
          return region.TwoLetterISORegionName;
        }
      }

      return null;
    }
  }

}
