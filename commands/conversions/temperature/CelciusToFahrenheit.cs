namespace CoarUtils.commands.conversions.temperature {
  public class CelciustoFahrenheit {
    public static decimal Execute(decimal c) {
      var f = 1.8M*c + 32M;
      return f;
    }
  }
}
