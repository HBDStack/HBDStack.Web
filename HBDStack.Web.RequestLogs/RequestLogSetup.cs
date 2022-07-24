using HBDStack.Web.RequestLogs;
using HBDStack.Web.RequestLogs.ApiRequests;
using HBDStack.Web.RequestLogs.HttpClients;
using HBDStack.Web.RequestLogs.Storage;
using Refit;
// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RequestLogSetup
    {
        public static IServiceCollection AddRequestStorage<TImplementation>(this IServiceCollection service) where TImplementation : class, IRequestLogStorage 
            => service.AddSingleton<IRequestLogStorage, TImplementation>();

        public static IServiceCollection AddRequestLogs(this IServiceCollection service, ApiRequestLoggingOptions options)
            => service.AddSingleton(Options.Options.Create(options))
                .AddScoped<IRequestLogService, RequestLogService>();
        
        /// <summary>
        /// Add Refit Interface with <see cref="RequestLogClientHandler"/>
        /// </summary>
        /// <param name="service"></param>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IHttpClientBuilder AddRefitClientWithLogs<T>(this IServiceCollection service, RequestLoggingOptions options)where T:class
        {
            service
                .AddSingleton(Options.Options.Create(options))
                .AddTransient<RequestLogClientHandler>();
            
            return service.AddRefitClient<T>().AddRequestLogClientHandler();
        }

        /// <summary>
        /// Register the <see cref="RequestLogClientHandler"/> into <see cref="IHttpClientBuilder"/>
        /// Ensure you already registered the <see cref="IOptions<RequestLoggingOptions>"/> before calling this method.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddRequestLogClientHandler(this IHttpClientBuilder builder)
            => builder.AddHttpMessageHandler<RequestLogClientHandler>();
    }
}

namespace Microsoft.AspNetCore.Builder
{
    public static class RequestLogBuilderSetup
    {
        public static IApplicationBuilder UseRequestLogs(this IApplicationBuilder builder)
            => builder.UseMiddleware<ApiRequestLogMiddleware>();
    }
}