using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Models;
using Shops.Tools;

namespace Shops.Services
{
    public class ShopManager
    {
        private readonly List<Shop> _shops;
        private readonly List<Product> _products;
        private int _nextProductId = 1;

        public ShopManager()
        {
            _shops = new List<Shop>();
            _products = new List<Product>();
        }

        public void RegisterShop(Shop shop)
        {
            _shops.Add(shop);
        }

        public Product RegisterProduct(string? name)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            if (name == string.Empty)
                throw new ShopsException("Name can't be empty");

            var product = new Product(_nextProductId, name);
            _nextProductId++;
            _products.Add(product);
            return product;
        }

        public IReadOnlyList<Shop> GetAllShops() => _shops.AsReadOnly();

        public IReadOnlyList<Product> GetAllProducts() => _products.AsReadOnly();

        public Shop? FindShopWithBestOffer(ProductOrder? order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));
            return FindShopWithBestOffer(new List<ProductOrder?>() { order });
        }

        public Shop? FindShopWithBestOffer(List<ProductOrder?>? orders)
        {
            ArgumentNullException.ThrowIfNull(orders);
            List<ProductOrder> validOrders = orders
                                                   .ThrowIfContainsNull(new ShopsException("Order can't be null"));

            return _shops.Where(s => s.HasEnoughProducts(validOrders))
                                    .MinBy(s => s.CalculateCost(validOrders));
        }
    }
}