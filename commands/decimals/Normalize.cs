namespace CoarUtils.commands.decimals {
  //https://stackoverflow.com/questions/4525854/remove-trailing-zeros
  public static class Normalize {
    //public static decimal Execute(this decimal value) {
    //  return value / 1.000000000000000000000000000000000m;
    //}

    public static decimal Execute(decimal value) {
      return value / 1.000000000000000000000000000000000m;
    }
  }
}

