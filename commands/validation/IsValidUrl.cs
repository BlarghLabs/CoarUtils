namespace CoarUtils.commands.validation {
  public class IsValidUrl {


    public static bool Execute(string candidateUrl) {
      if (string.IsNullOrWhiteSpace(candidateUrl)) {
        return false;
      }

      var isValidUrl =
        Uri.TryCreate(candidateUrl, UriKind.Absolute, out var uriResult)
        &&
        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
      ;
      return isValidUrl;
    }
  }
}