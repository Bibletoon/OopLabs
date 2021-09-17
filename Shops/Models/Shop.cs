using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Tools;

namespace Shops.Models
{
    public class Shop
    {
        private readonly Dictionary<int, Lot> _availableProducts;

        public Shop(string name)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            if (name == string.Empty)
                throw new ShopsException("Name can't be empty");

            Name = name;
            _availableProducts = new Dictionary<int, Lot>();
        }

        public string Name { get; init; }

        public void AddLots(List<Lot> lots)
        {
            ArgumentNullException.ThrowIfNull(lots, nameof(lots));
            List<Lot> notNullLots = lots.ThrowIfContainsNull(new ShopsException("Lot can't be null"));

            if (!notNullLots.All(CanAddLot))
            {
                var errorLot = notNullLots.First(l => !CanAddLot(l));
                throw new ShopsException($"Can't add product {errorLot.Product.Name}. Too big count provided.");
            }

            foreach (Lot lot in notNullLots)
            {
                AddLot(lot);
            }
        }

        public void AddLot(Lot lot)
        {
            ArgumentNullException.ThrowIfNull(lot, nameof(lot));

            if (!CanAddLot(lot))
            {
                throw new ShopsException($"Can't add product {lot.Product.Name}. Too big count provided.");
            }

            if (!_availableProducts.TryGetValue(lot.Product.Id, out Lot existingLot))
            {
                _availableProducts[lot.Product.Id] = new Lot(lot.Product, lot.Count, lot.Price);
                return;
            }

            if (existingLot.Count == 0)
                existingLot.SetPrice(lot.Price);

            existingLot.IncreaseCount(lot.Count);
        }

        public IReadOnlyList<Lot> GetAllProductsInfo()
            => _availableProducts.Values.Where(p => p.Count > 0).ToList();

        public Lot GetProductInfo(Product product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));
            if (_availableProducts.TryGetValue(product.Id, out Lot existingLot)
                && existingLot.Count > 0)
                return _availableProducts[product.Id];

            throw new ShopsException("Product isn't presented at shop");
        }

        public void SetProductPrice(Product product, int price)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            if (price <= 0)
                throw new ShopsException("Price can't be less than or equal zero");

            if (!_availableProducts.TryGetValue(product.Id, out Lot existingLot) || existingLot.Count <= 0)
                throw ShopsException.ProductIsNotPresentedAtShop(product, this);

            existingLot.SetPrice(price);
        }

        public void Buy(User customer, ProductOrder order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));
            Buy(customer, new List<ProductOrder>()
            {
                order,
            });
        }

        public void Buy(User customer, List<ProductOrder> orders)
        {
            ArgumentNullException.ThrowIfNull(customer, nameof(customer));
            ArgumentNullException.ThrowIfNull(orders, nameof(orders));
            List<ProductOrder> notNullOrders = orders
                .ThrowIfContainsNull(new ShopsException("Order can't be null"));

            bool ordersAreUnique = notNullOrders.Select(order => order.Product.Id).Distinct().Count() == orders.Count;

            if (!ordersAreUnique)
                throw new ShopsException("Order can't have similar products");

            bool canProvideEnoughProducts = notNullOrders.All(HasEnoughProducts);

            if (!canProvideEnoughProducts)
                throw new ShopsException($"Not enough products in shop {Name}");

            int totalCost = notNullOrders.Sum(CalculateOrderCost);

            if (customer.Money < totalCost)
                throw new ShopsException($"User {customer.Name} has not enough money");

            customer.PayMoney(totalCost);
            SellProducts(notNullOrders);
        }

        internal bool HasEnoughProducts(List<ProductOrder> order) =>
            order.All(HasEnoughProducts);

        internal long CalculateCost(List<ProductOrder> orders) =>
            orders.Sum(CalculateOrderCost);

        private bool HasEnoughProducts(ProductOrder order) =>
                _availableProducts.TryGetValue(order.Product.Id, out Lot existingLot)
                && existingLot.Count >= order.Count;

        private int CalculateOrderCost(ProductOrder order) =>
                order.Count * _availableProducts[order.Product.Id].Price;

        private void SellProducts(List<ProductOrder> orders)
        {
            foreach (ProductOrder order in orders)
            {
                _availableProducts[order.Product.Id].DecreaseCount(order.Count);
            }
        }

        private bool CanAddLot(Lot lot)
        {
            if (!_availableProducts.TryGetValue(lot.Product.Id, out Lot existingLot))
                return true;

            return existingLot.CanIncreaseCount(lot.Count);
        }
    }
}