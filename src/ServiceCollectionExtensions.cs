using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;

namespace Sufficit.Gateway.ReceitaNet
{
    /// <summary>
    /// Classe que controla os metodos referentes a API de Endpoints da Sufficit
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Inclui toda a configuração para sincronia com a API de EndPoints da Sufficit
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddGatewayReceitaNet(this IServiceCollection services)
        {
            services.AddOptions<GatewayOptions>();

            var provider = services.BuildServiceProvider(false);
            var configuration = provider.GetRequiredService<IConfiguration>();

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<GatewayOptions>(configuration.GetSection(GatewayOptions.SECTIONNAME));

            // Capturando para uso local
            var options = configuration.GetSection(GatewayOptions.SECTIONNAME).Get<GatewayOptions>() ?? new GatewayOptions();

            services.AddTransient<ProtectedApiQueryTokenHandler>();
            services.AddHttpClient(options.ClientId, client => client.Configure(options)).AddHttpMessageHandler<ProtectedApiQueryTokenHandler>();

            services.AddSingleton<APIClientService>();
            return services;
        }
    }
}
