using Microsoft.OpenApi.Interfaces;

namespace HBDStack.Web.Swagger;

public interface IApiDocExtension : IOpenApiExtension
{
    string Name { get; }
    
}