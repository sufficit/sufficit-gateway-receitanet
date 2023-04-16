using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Gateway.ReceitaNet.Parameters;
using Sufficit.Gateway.ReceitaNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public class APIClientService : ControllerSection
    {
        public const string APPLICATION = "ura";

        public APIClientService(IOptionsMonitor<GatewayOptions> ioptions, IHttpClientFactory clientFactory, ILogger<APIClientService> logger)
            : base(ioptions, clientFactory, logger, Json.Options)
        {     
            logger.LogTrace($"Sufficit ReceitaNet Gateway API Client Service instantiated with base address: {options.BaseUrl}");
        }

        public Task<ContractResponse> GetContract(GetContractParameters parameters, string token, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("get client by parameters: {parameters}", parameters);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["token"] = token;
            query["app"] = APPLICATION;

            var uri = new Uri($"clientes?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request<ContractResponse>(message, cancellationToken);
        }

        public Task<NotifyResponse> PaymentNotification(ContractAndContactParameters parameters, string token, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("payment notification by parameters: {parameters}", parameters);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["token"] = token;
            query["app"] = APPLICATION;

            var uri = new Uri($"notificacao-pagamento?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request<NotifyResponse>(message, cancellationToken);
        }

        public Task<ChargeResponse> ChargeNotification(ChargeNotificationParameters parameters, string token, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("charge notification by parameters: {parameters}", parameters);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["token"] = token;
            query["app"] = APPLICATION;

            var uri = new Uri($"boletos?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request<ChargeResponse>(message, cancellationToken);
        }

        public async Task<ConnectionStatusResponse> GetConnectionStatus(ContractAndContactParameters parameters, string token, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("get connection status by parameters: {parameters}", parameters);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["token"] = token;
            query["app"] = APPLICATION;

            var uri = new Uri($"verificar-acesso?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            try
            {
                return await Request<ConnectionStatusResponse>(message, cancellationToken);
            } 
            catch(HttpRequestException ex) 
            {
                int statuscode;
#if NET5_0_OR_GREATER
                statuscode = (int)ex.StatusCode.GetValueOrDefault();
#else
                statuscode = ex.Message.ToLowerInvariant().Contains("server error") ? 500 : -1;
#endif
                if (statuscode == 500)                
                    logger.LogWarning(ex, "error on getting connection status, probably the client has no server configured at receitanet");                
                else                
                    logger.LogError(ex, "error ({code}) on getting connection status: {message}", statuscode, ex.Message);                    
                
                return new ConnectionStatusResponse()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Open a ticket
        /// </summary>
        public Task<TicketResponse> Ticket(TicketParameters parameters, string token, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("open ticket by parameters: {parameters}", parameters);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["token"] = token;
            query["app"] = APPLICATION;

            var uri = new Uri($"abertura-chamado?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request<TicketResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Update a recording for a ticket
        /// </summary>
        public Task<RecordingResponse> Recording(RecordingParameters parameters, string token, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("update ticket recording by parameters: {parameters}", parameters);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["token"] = token;
            query["app"] = APPLICATION;

            var uri = new Uri($"chamado-gravacao?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request<RecordingResponse>(message, cancellationToken);
        }
    }
}
