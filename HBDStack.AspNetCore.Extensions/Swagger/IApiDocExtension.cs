using Microsoft.OpenApi.Interfaces;

namespace HBDStack.AspNetCore.Extensions.Swagger;

public interface IApiDocExtension : IOpenApiExtension
{
    string Name { get; }
    
}