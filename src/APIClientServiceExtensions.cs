using Sufficit.Gateway.ReceitaNet.Parameters;
using Sufficit.Gateway.ReceitaNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public static class APIClientServiceExtensions
    {
        public static Task<ContractResponse> GetContractByPhone(this APIClientService source, string token, string phone, CancellationToken cancellationToken = default)
        {
            var parameters = new GetContractParameters();
            parameters.Phone = phone;
            return source.GetContract(parameters, token, cancellationToken);
        }

        public static Task<ContractResponse> GetContractByDocument(this APIClientService source, string token, string document, CancellationToken cancellationToken = default)
        {
            var parameters = new GetContractParameters();
            parameters.Document = document;
            return source.GetContract(parameters, token, cancellationToken);
        }

        /// <summary>
        /// Payment notify
        /// </summary>
        public static Task<NotifyResponse> PaymentNotification(this APIClientService source, string token, int contract, string? contact = null, CancellationToken cancellationToken = default)
        {
            var parameters = new ContractAndContactParameters();
            parameters.ContractId = contract;
            parameters.Contact = contact;
            return source.PaymentNotification(parameters, token, cancellationToken);
        }
    }
}
