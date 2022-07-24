using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HBDStack.AspNetCore.Extensions.Swagger;

public class SwaggerSecurityDefinitionHelper
{
    #region Fields

    private readonly SwaggerGenOptions _options;

    #endregion Fields

    #region Constructors

    internal SwaggerSecurityDefinitionHelper(SwaggerGenOptions options) => _options = options;

    #endregion Constructors

    #region Methods

    internal void AddSecurityDefinition(string name, OpenApiSecurityScheme scheme) => _options.AddSecurityDefinition(name, scheme);

    internal void AddSecurityRequirement(OpenApiSecurityRequirement requirement) => _options.AddSecurityRequirement(requirement);

    #endregion Methods
}