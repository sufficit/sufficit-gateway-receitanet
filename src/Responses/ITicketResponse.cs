using System;
using System.Collections.Generic;
using System.Text;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public interface ITicketResponse
    {
        bool Success { get; }

        string Protocol { get; } 

        int TicketId { get; }
    }
}
