using Microsoft.OpenApi.Any;

namespace HBDStack.AspNetCore.Extensions.Swagger;

internal class XLogoExtention : OpenApiObject, IApiDocExtension
{
    #region Constructors

    public XLogoExtention(XLogoOptions options)
    {
        Add("url", new OpenApiString(options.Url));
        Add("altText", new OpenApiString(options.AltText));
        Add("backgroundColor", new OpenApiString(options.BackgroundColor));
        Add("href", new OpenApiString(options.Href));
    }

    #endregion Constructors

    #region Properties

    public string Name => "x-logo";

    #endregion Properties
}