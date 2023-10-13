using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingEngineServer.Orders
{
    public class OrderCore : IOrderCore
    {
        public OrderCore(long orderId, string username, int securityID) 
        {
            OrderID = orderId;
            Username = username;
            SecurityID = securityID;
        }

        public long OrderID { get; private set; }
        public int SecurityID { get; private set; }
        public string Username { get; private set; }
        
    }
}
