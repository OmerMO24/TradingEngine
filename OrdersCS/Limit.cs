using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingEngineServer.Orders
{
    public class Limit
    {

        public Limit(long price)
        {
            Price = price;
        }
        public long Price { get; private set; }
        public OrderBookEntry Head {get; set; }
        public OrderBookEntry Tail { get; set; }
        public uint GetLevelOrderCount()
        {
            uint orderCount = 0;
            OrderBookEntry headPointer = Head;
            while (headPointer != null)
            {
                if (headPointer.CurrentOrder.CurrentQuanitity != 0)
                    orderCount++;
                headPointer = headPointer.Next;
            }
            return orderCount;
        }

        public uint GetLevelOrderQuantity()
        {
            uint orderQuantity = 0;
            OrderBookEntry headPointer = Head;
            while (headPointer != null) 
            {
                orderQuantity += headPointer.CurrentOrder.CurrentQuanitity;
                headPointer = headPointer.Next;
            }
            return orderQuantity;
        }

        public List<OrderRecord> GetLevelOrderRecords()
        {
            List<OrderRecord> orderRecords = new List<OrderRecord>();   
            OrderBookEntry headPointer = Head;
            uint theoreticalQueuePosition = 0;
            while (headPointer != null) 
            {
                var currentOrder = headPointer.CurrentOrder;
                if (currentOrder.CurrentQuanitity != 0)
                    orderRecords.Add(new OrderRecord(currentOrder.OrderID, currentOrder.CurrentQuanitity,
                        Price, currentOrder.IsBuySide, currentOrder.Username, currentOrder.SecurityID,
                        theoreticalQueuePosition)); 
                theoreticalQueuePosition++;
                Head = headPointer.Next;
            }

            return orderRecords;

        }

        public bool IsEmpty
        {
            get
            {
                return Head == null && Tail == null;
            }
        }
        public Side Side
        {
            get
            {
                if (IsEmpty)
                    return Side.Unknown;
                else
                {
                    return Head.CurrentOrder.IsBuySide ? Side.Bid : Side.Ask;
                }
            }
        }
    }
}
