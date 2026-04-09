#if NET7_0_OR_GREATER
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sufficit.Gateway.ReceitaNet.MCP;
using System;

namespace Sufficit.Gateway.ReceitaNet
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the dedicated ReceitaNet MCP surface and ensures the required ReceitaNet client graph exists.
        /// </summary>
        /// <param name="services">Application service collection.</param>
        /// <param name="configure">Optional callback to override MCP options programmatically.</param>
        /// <returns>The same service collection for chaining.</returns>
        public static IServiceCollection AddGatewayReceitaNetMCP(this IServiceCollection services, Action<ReceitaNetMcpOptions>? configure = null)
        {
            services.AddOptions<ReceitaNetMcpOptions>();

            var provider = services.BuildServiceProvider(false);
            var configuration = provider.GetRequiredService<IConfiguration>();
            services.Configure<ReceitaNetMcpOptions>(configuration.GetSection(ReceitaNetMcpOptions.SECTIONNAME));

            if (configure != null)
                services.Configure(configure);

            services.AddHttpContextAccessor();
            services.TryAddSingleton<ReceitaNetMcpSessionManager>();
            services.TryAddSingleton<ReceitaNetMcpTokenResolver>();
            services.TryAddSingleton<ReceitaNetMcpToolCatalog>();
            return services;
        }

        /// <summary>
        /// Registers the ReceitaNet gateway client services together with the dedicated MCP surface.
        /// </summary>
        /// <param name="services">Application service collection.</param>
        /// <param name="configure">Optional callback to override MCP options programmatically.</param>
        /// <returns>The same service collection for chaining.</returns>
        public static IServiceCollection AddGatewayReceitaNetWithMCP(this IServiceCollection services, Action<ReceitaNetMcpOptions>? configure = null)
        {
            services.AddGatewayReceitaNet();
            return services.AddGatewayReceitaNetMCP(configure);
        }
    }
}
#endif