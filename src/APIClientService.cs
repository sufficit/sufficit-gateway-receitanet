using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Gateway.ReceitaNet.Parameters;
using Sufficit.Gateway.ReceitaNet.Responses;
using System;
using System.Net.Http;
using System.Net.Http.Json;
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
            logger.LogTrace("Sufficit ReceitaNet Gateway API Client Service instantiated with base address: {baseurl}", options.BaseUrl);
        }

        public Task<ContractResponse> GetContract(GetContractParameters parameters, string token, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("get client by contract: {contract}, document: {document}, phone: {phone}", 
                parameters.ContractId, 
                parameters.Document,
                parameters.Phone);

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
            logger.LogTrace("payment notification by contract: {contract}, contact: {contact}", 
                parameters.ContractId, 
                parameters.Contact);

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
            logger.LogTrace("charge notification by contract: {contract}, contact: {contact}, channel: {channel}", 
                parameters.ContractId,
                parameters.Contact,
                parameters.Channel);

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
            logger.LogTrace("get connection status by contract: {contract}, contact: {contact}", 
                parameters.ContractId, 
                parameters.Contact);

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
            catch (Exception ex) 
            {
                if (ex is HttpRequestException httpException)
                {
                    int statuscode;
                    #if NET5_0_OR_GREATER
                        statuscode = (int)httpException.StatusCode.GetValueOrDefault();
                    #else
                        statuscode = httpException.Message.ToLowerInvariant().Contains("server error") ? 500 : -1;
                    #endif

                    if (statuscode == 500)
                    {
                        ex = new MissingServerConfigurationException(parameters.ContractId, ex);
                        logger.LogWarning(ex, "error on getting connection status, {message}", ex.Message);
                    }
                    else
                    {
                        logger.LogError(ex, "error ({code}) on getting connection status: {message}", statuscode, ex.Message);
                    }
                }

                return new ConnectionStatusResponse()
                {
                    Success = false,
                    Message = ex.Message,
                    Exception = ex
                };
            }
        }

        /// <summary>
        /// Open a ticket
        /// </summary>
        public Task<TicketResponse> Ticket(TicketParameters parameters, string token, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("open ticket by contract: {contract}, contact: {contact}, kind: {kind}, reason: {reason}", 
                parameters.ContractId,
                parameters.Contact,
                parameters.Kind,
                parameters.Reason);

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
            logger.LogTrace("update ticket recording by ticket: {ticket}, extension: {extension}, recording: {recording}, finalize: {finalize}", 
                parameters.TicketId,
                parameters.Extension,
                parameters.Recording,
                parameters.Finalize);

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
