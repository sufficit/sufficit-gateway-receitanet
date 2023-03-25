using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public class Contract
    {
        public Contract() 
        {
            Tickets = new ContractTickets();
            Server = new ServerInfo();
            Invoices = new HashSet<InvoiceItem>();
        }

        /// <summary>
        /// (required) código do cliente
        /// </summary>
        [JsonPropertyName("idCliente")]
        public int ClientId { get; set; }

        /// <summary>
        /// (required) nome do cliente
        /// </summary>
        [JsonPropertyName("razaoSocial")]
        public string Title { get; set; } = default!;

        /// <summary>
        /// contract status string representation (Ativo, Suspenso)
        /// </summary>
        [JsonPropertyName("contratoStatusDisplay")]
        public string StatusDisplay { get; set; } = default!;

        /// <summary>
        /// (required) status do cliente promessa de pagamneto excedido (0) ou não (1)
        /// </summary>
        [JsonPropertyName("isPromessaPagamento")]
        public bool Promessa { get; set; }

        /// <summary>
        /// (required) valor do último boleto não pago
        /// </summary>
        [JsonPropertyName("contratoValorAberto")]
        public decimal ValorAberto { get; set; }

        /// <summary>
        /// itens de faturamento (array of assets.ItemFatura)
        /// </summary>
        [JsonPropertyName("faturasEmAberto")]
        public ICollection<InvoiceItem> Invoices { get; }

        /// <summary>
        /// (required) cpf/cnpj do cliente
        /// </summary>
        [JsonPropertyName("cpfCnpj")]
        public string Document { get; set; } = default!;

        /// <summary>
        /// contract status enum
        /// </summary>
        [JsonPropertyName("contratoStatus")]
        public int Status { get; set; }

        /// <summary>
        /// existe chamado aberto para o cliente ?
        /// </summary>
        [JsonPropertyName("existeChamado")]
        public int HasTicket { get; set; }

        /// <summary>
        /// status do cliente (Ativo, Suspenso)
        /// </summary>
        [JsonPropertyName("isChamados")]
        public ContractTickets? Tickets { get; }

        /// <summary>
        /// cliente possui e-mail válido para envio de cobrança ?
        /// </summary>
        [JsonPropertyName("isEmail")]
        public bool HasEMail { get; set; }

        /// <summary>
        /// cliente possui telefone válido para envio de cobrança por sms ?
        /// </summary>
        [JsonPropertyName("isSMS")]
        public bool HasSMS { get; set; }


        [JsonPropertyName("tecnologia")]
        public int Technology { get; set; }

        [JsonPropertyName("servidor")]
        public ServerInfo Server { get; }

        /// <summary>
        /// Contract has any open or expired invoice
        /// </summary>
        public bool IsPending 
            => Invoices?.Any() ?? false;

        public bool HasExpired
            => Expired?.Any() ?? false;

        public IEnumerable<InvoiceItem> Expired
        {
            get
            {
                if (Invoices != null)
                {
                    foreach (var item in Invoices)
                    {
                        if (item.Vencimento.ToUniversalTime() < DateTime.UtcNow.AddDays(-1))
                        {
                            yield return item;
                        }                        
                    }
                }
            }
        }
    }
}
