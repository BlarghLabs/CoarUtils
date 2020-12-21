using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace CoarUtils.filters {
  public class RemoveVersionParameterFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
      var vp = operation.Parameters.FirstOrDefault(p => p.Name == "version");
      if (vp != null) {
        operation.Parameters.Remove(vp);
      }
      //var toReplaceWith = new OpenApiPaths();
      //foreach (var (key, value) in operation.Paths) {
      //  toReplaceWith.Add(key.Replace("v{version}", operation.Info.Version, StringComparison.InvariantCulture), value);
      //}

      //operation.Paths = toReplaceWith;
    }
  }
}
