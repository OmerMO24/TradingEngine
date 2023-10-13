using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TradingEngineServer.Instrument;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public class Orderbook : IRetrievalOrderbook
    {
        // PRIVATE FIELDS //
        private readonly Security _instrument;
        private readonly Dictionary<long, OrderBookEntry> _orders = new Dictionary<long, OrderBookEntry>();
        private readonly SortedSet<Limit> _askLimits = new SortedSet<Limit>(AskLimitComparer.Comparer);
        private readonly SortedSet<Limit> _bidLimits = new SortedSet<Limit>(BidLimitComparer.Comparer);
        

        public Orderbook(Security instrument)
        {
            _instrument = instrument;
        }

        public int Count => _orders.Count;

        public void AddOrder(Order order)
        {
            var baseLimit = new Limit(order.Price);
            AddOrder(order, baseLimit, order.IsBuySide ? _bidLimits : _askLimits, _orders);
        }

        private static void AddOrder(Order order, Limit baseLimit, SortedSet<Limit> limitLevels, Dictionary<long, OrderBookEntry> internalBook)
        {
            if (limitLevels.TryGetValue(baseLimit, out Limit limit))
            {
                OrderBookEntry orderbookEntry = new OrderBookEntry(order, baseLimit);
                if (limit.Head == null)
                {
                    limit.Head = orderbookEntry;
                    limit.Tail = orderbookEntry;

                }
                else 
                {
                    OrderBookEntry tailPointer = limit.Tail;
                    tailPointer.Next = orderbookEntry;
                    orderbookEntry.Previous = tailPointer;
                    limit.Tail = orderbookEntry;
                }
                internalBook.Add(order.OrderID, orderbookEntry);

            }
            else
            {
                limitLevels.Add(baseLimit);
                OrderBookEntry orderbookEntry = new OrderBookEntry(order, baseLimit);
                baseLimit.Head = orderbookEntry;
                baseLimit.Tail = orderbookEntry;
                internalBook.Add(order.OrderID, orderbookEntry);
                
            }

        }
        public void CancelOrder(CancelOrder cancelOrder)
        {
            throw new NotImplementedException();
        }

        public void ChangeOrder(ModifyOrder modifyOrder)
        {
            if (_orders.TryGetValue(modifyOrder.OrderID, out OrderBookEntry obe))
            {
                RemoveOrder(modifyOrder.ToCancelOrder());
                AddOrder(modifyOrder.ToNewOrder(), obe.ParentLimit, modifyOrder.IsBuySide ? _bidLimits : _askLimits, _orders);
            }
        }

        private void RemoveOrder(CancelOrder cancelOrder)
        {
            if (_orders.TryGetValue(cancelOrder.OrderID, out var obe))
            {
                RemoveOrder(cancelOrder.OrderID, obe, _orders);
            }
        }

        public bool ContainsOrder(long orderId)
        {
            return _orders.ContainsKey(orderId);    
        }

        public List<OrderBookEntry> GetAskOrders()
        {
            List<OrderBookEntry> orderBookEntries = new List<OrderBookEntry>(); 
            foreach (var askLimit in _askLimits)
            {
                if (askLimit.IsEmpty)
                    continue;
                else
                {
                    OrderBookEntry askLimitPointer = askLimit.Head;
                    while (askLimitPointer != null)
                    {
                        orderBookEntries.Add(askLimitPointer);
                        askLimitPointer = askLimitPointer.Next;
                    }
                }
            }
            return orderBookEntries;
                
        }

        public List<OrderBookEntry> GetBidOrders()
        {
            List<OrderBookEntry> orderBookEntries = new List<OrderBookEntry>();
            foreach (var bidLimits in _bidLimits)
            {
                if (bidLimits.IsEmpty)
                    continue;
                else
                {
                    OrderBookEntry bidLimitPointer = bidLimits.Head;
                    while (bidLimitPointer != null)
                    {
                        orderBookEntries.Add(bidLimitPointer);
                        bidLimitPointer = bidLimitPointer.Next;
                    }
                }
            }
            return orderBookEntries;
        }

        public OrderbookSpread GetSpread()
        {
            long? bestAsk = null, bestBid = null;
            if (_askLimits.Any() && !_askLimits.Min.IsEmpty)
                bestAsk = _askLimits.Min.Price;
            if (_bidLimits.Any() && !_bidLimits.Min.IsEmpty)
                bestBid = _bidLimits.Max.Price;
            return new OrderbookSpread(bestBid, bestAsk);
        }


        private static void RemoveOrder(long orderID, OrderBookEntry obe, Dictionary<long, OrderBookEntry> internalBook)
        {
            // Deal with location of ORderbookEntry within the LinkedList
            if (obe.Previous != null && obe.Next != null)
            {
                obe.Next.Previous = obe.Previous;
                obe.Previous.Next = obe.Next;
            }
            else if (obe.Previous != null)
            {
                obe.Previous.Next = null;
            }
            else if (obe.Next != null)
            {
                obe.Next.Previous = null;
            }

            // Deal with OrderBookEntry on Limit Level
            if (obe.ParentLimit.Head == obe && obe.ParentLimit.Tail == obe)
            {
                //One order on this level
                obe.ParentLimit.Head = null;
                obe.ParentLimit.Tail = null;

            }
            else if (obe.ParentLimit.Head != null)
            {

                //More than one order, but obe is first order.
                obe.ParentLimit.Head = obe.Next;
            }
            else if (obe.ParentLimit.Tail == obe)
            {
                //More than one order, but obe is last order on level
                obe.ParentLimit.Tail = obe.Previous;
            }

            internalBook.Remove(orderID);
        }
    }
}
