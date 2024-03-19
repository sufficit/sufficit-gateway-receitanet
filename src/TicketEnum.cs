using System;
using System.Collections.Generic;
using System.Text;

namespace Sufficit.Gateway.ReceitaNet
{
    public enum TicketEnum
    {
        /// <summary>
        ///     Used for connection and service problems tickets
        /// </summary>
        Maintenance = 1,

        /// <summary>
        ///     Used for installations or equipament changes | upgrades tickets
        /// </summary>
        Installation = 2,

        /// <summary>
        ///     Used for wi-fi signal problems
        /// </summary>
        Sight = 3,

        /// <summary>
        ///     Used after a cancellation for equipament removal
        /// </summary>
        EquipmentRemoval = 4,

        /// <summary>
        ///     Used for financial problems tickets, like a new bankslip or any expired billing
        /// </summary>
        Financial = 5
    }
}
