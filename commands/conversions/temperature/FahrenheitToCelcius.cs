namespace CoarUtils.commands.conversions.temperature {
  public class FahrenheitToCelcius {
    public static decimal Execute(decimal f) {
      var c = (f - 32M) * (5M / 9M);
      return c;
    }
  }
}


