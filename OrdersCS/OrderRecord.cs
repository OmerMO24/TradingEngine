using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingEngineServer.Orders
{
    
        /// <summary> 
        /// Read-only representation of an order.
        /// </summary>

        public record OrderRecord(long OrderId, uint Quantity, long Price, 
            bool IsBuySide, string Username, int SecurityID, uint TheoreticalQueuePosition);
        
    
}



