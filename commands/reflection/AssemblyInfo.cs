using System;
using System.Reflection;

namespace CoarUtils.Utils {
  public class AssemblyInfo {
    /// <summary>
    /// Gets the assembly product.
    /// </summary>
    /// <value>The assembly product.</value>
    public static string AssemblyProduct {
      get {
        // Get all Product attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
        // If there aren't any Product attributes, return an empty string
        if (attributes.Length == 0)
          return "";
        // If there is a Product attribute, return its value
        return ((AssemblyProductAttribute)attributes[0]).Product;
      }
    }
    public static string AssemblyCompany {
      get {
        // Get all Product attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        // If there aren't any Product attributes, return an empty string
        if (attributes.Length == 0)
          return "";
        // If there is a Product attribute, return its value
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }
    public static string AssemblyName {
      get {
        //made this lame
        return Assembly.GetExecutingAssembly().GetName().Name.Trim().Replace(" ", "");
      }
    }
    public static string ExecutingAssemblyPath {
      get {
        //made this lame
        return Assembly.GetExecutingAssembly().CodeBase.ToLower().Replace("\\", "/");
      }
    }

    public static Version AssemblyVersion {
      get {
        //made this lame
        return Assembly.GetExecutingAssembly().GetName().Version;
      }
    }

    public static string VersionBuildNumber {
      get {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
      }
    }
  }
}
