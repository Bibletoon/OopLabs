using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Models;
using Shops.Tools;

namespace Shops.Services
{
    public class ShopManager
    {
        private int _nextProductId = 1;
        private List<Shop> _shops;

        public ShopManager()
        {
            _shops = new List<Shop>();
        }

        public Shop Create(string name)
        {
            var newShop = new Shop(name);
            _shops.Add(newShop);
            return newShop;
        }

        public Product RegisterProduct(string name)
        {
            var product = new Product(_nextProductId, name);
            _nextProductId++;
            return product;
        }

        public Shop? FindShopWithBestOffer(ProductOrder? order)
        {
            order = order.ThrowIfNull(new ArgumentNullException(nameof(order)));
            return FindShopWithBestOffer(new List<ProductOrder?>() { order });
        }

        public Shop? FindShopWithBestOffer(List<ProductOrder?>? orders)
        {
            List<ProductOrder> validOrders = orders.ThrowIfNull(new ArgumentNullException(nameof(orders)))
                                                   .ThrowIfContainsNull(new ShopsException("Order can't be null"));

            var shops = _shops.Where(s => s.HasEnoughProducts(validOrders)).ToList();

            if (shops.Count == 0)
                return null;

            return shops.MinBy(s => s.CalculateCost(validOrders));
        }
    }
}