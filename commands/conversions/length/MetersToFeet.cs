namespace CoarUtils.commands.conversions.length {
  public class MetersToFeet {
    public static decimal Execute(decimal meters) {
      //https://www.google.com/search?q=meters+to+feet+high+precision&rlz=1C1CHBF_enUS923US923&sxsrf=APq-WBufHeMIv9f6XYH-jfJ0rwpW3LkZUA%3A1648070261002&ei=dI47YsbrPP3XytMP9smMkA0&ved=0ahUKEwjG9eKTlN32AhX9q3IEHfYkA9IQ4dUDCA4&uact=5&oq=meters+to+feet+high+precision&gs_lcp=Cgdnd3Mtd2l6EAMyBggAEAgQHjIGCAAQCBAeOgcIIxCwAxAnOgcIABBHELADOgYIABAHEB46CAgAEAgQBxAeSgQIQRgASgQIRhgAUMAKWI4jYLMkaAJwAXgAgAFDiAHVBJIBAjEwmAEAoAEByAEJwAEB&sclient=gws-wiz
      //var feet = meters * 3.280839895M;
      //https://www.sfei.org/it/gis/map-interpretation/conversion-constants
      var feet = meters * 3.28083333333M;
      return feet;
    }
  }
}
