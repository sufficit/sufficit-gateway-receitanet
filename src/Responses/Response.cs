using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public class Response
    {
        ///<summary> 
        ///<para>Indica se houve algum erro de processamento interno no destino</para>
        ///<para>Não se refere ao resultado da consulta</para>
        ///<para>EX: Não indica se o cliente esta conectado</para>
        ///</summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("msg")]
        public string? Message { get; set; }
    }
}
