using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingEngineServer.Orders
{
    public class Order : IOrderCore
    {
        public Order(IOrderCore orderCore, long price, uint quantity, bool isBuySide) 
        {

            // PROPERTIES //
            Price = price;  
            IsBuySide = isBuySide;  
            InitialQuantity = quantity;
            CurrentQuanitity = quantity;

            // FIELDS //
            _orderCore = orderCore;
        }

        public Order(ModifyOrder modifyOrder) : 
            this(modifyOrder, modifyOrder.Price, modifyOrder.Quantity, modifyOrder.IsBuySide)
        {
            
        }

        // PROPERTIES //
        public long Price { get; private set; }
        public uint InitialQuantity { get; private set; }
        public uint CurrentQuanitity { get; private set; }
        public bool IsBuySide { get; private set; }

        public long OrderID => _orderCore.OrderID;

        public string Username => _orderCore.Username;

        public int SecurityID => _orderCore.SecurityID;

        // METHODS //
        public void IncreaseQuantity(uint quantityDelta)
        {
            CurrentQuanitity += quantityDelta;
        }

        public void DecreaseQuantity(uint quantityDelta)
        {
            if (quantityDelta > CurrentQuanitity)
                throw new InvalidOperationException($"Quantity Delta > Current Quantity for OrderID={OrderID}");
            CurrentQuanitity -= quantityDelta;
        }

        // FIELDS //    
        private readonly IOrderCore _orderCore;
    
    }
}
