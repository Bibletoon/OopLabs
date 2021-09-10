﻿using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Tools;

namespace Shops.Models
{
    public class Shop
    {
        private readonly Dictionary<int, Lot> _availableProducts;

        internal Shop(string name)
        {
            Name = name;
            _availableProducts = new Dictionary<int, Lot>();
        }

        public string Name { get; init; }

        public void AddLots(List<Lot?>? lots)
        {
            List<Lot> notNullLots = lots.ThrowIfNull(new ArgumentNullException(nameof(lots)))
                                               .ThrowIfContainsNull(new ShopsException("Lot can't be null"));

            foreach (Lot lot in notNullLots)
            {
                AddLot(lot);
            }
        }

        public void AddLot(Lot lot)
        {
            lot = lot.ThrowIfNull(new ArgumentNullException(nameof(lot)));

            if (!_availableProducts.ContainsKey(lot.Product.Id))
            {
                _availableProducts[lot.Product.Id] = new Lot(lot.Product, lot.Count, lot.Price);
                return;
            }

            if (_availableProducts[lot.Product.Id].Count == 0)
                _availableProducts[lot.Product.Id].SetPrice(lot.Price);

            _availableProducts[lot.Product.Id].IncreaseCount(lot.Count);
        }

        public Lot GetProductInfo(Product? product)
        {
            product = product.ThrowIfNull(new ArgumentNullException(nameof(product)));
            if (_availableProducts.ContainsKey(product.Id)
                && _availableProducts[product.Id].Count > 0)
                return _availableProducts[product.Id];

            throw new ShopsException("Product isn't presented at shop");
        }

        public void SetProductPrice(Product? product, int price)
        {
            product = product.ThrowIfNull(new ArgumentNullException(nameof(product)));

            if (price <= 0)
                throw new ShopsException("Price can't be less than or equal zero");

            if (!_availableProducts.ContainsKey(product.Id) || _availableProducts[product.Id].Count <= 0)
                throw ShopsException.ProductIsNotPresentedAtShop(product, this);

            _availableProducts[product.Id].SetPrice(price);
        }

        public void Buy(Person? customer, ProductOrder? order)
        {
            order = order.ThrowIfNull(new ArgumentNullException(nameof(order)));
            Buy(customer, new List<ProductOrder?>()
            {
                order,
            });
        }

        public void Buy(Person? customer, List<ProductOrder?>? nullableOrders)
        {
            customer = customer.ThrowIfNull(new ArgumentNullException(nameof(customer)));
            List<ProductOrder> orders = nullableOrders
                                        .ThrowIfNull(new ArgumentNullException(nameof(orders)))
                                        .ThrowIfContainsNull(new ShopsException("Order can't be null"));

            bool ordersAreUnique = orders.Select(order => order.Product.Id).Distinct().Count() == orders.Count;

            if (!ordersAreUnique)
                throw new ShopsException("Order can't have similar products");

            bool canProvideEnoughProducts = orders.All(HasEnoughProducts);

            if (!canProvideEnoughProducts)
                throw new ShopsException($"Not enough products in shop {Name}");

            long totalCost = orders.Sum(CalculateOrderCost);

            if (customer.Money < totalCost)
                throw new ShopsException($"Person {customer.Name} has not enough money");

            customer.PayMoney(totalCost);
            SellProducts(orders);
        }

        internal bool HasEnoughProducts(List<ProductOrder> order) =>
            order.All(HasEnoughProducts);

        internal long CalculateCost(List<ProductOrder> orders) =>
            orders.Sum(CalculateOrderCost);

        private bool HasEnoughProducts(ProductOrder order) =>
                _availableProducts.ContainsKey(order.Product.Id)
                && _availableProducts[order.Product.Id].Count >= order.Count;

        private int CalculateOrderCost(ProductOrder order) =>
                order.Count * _availableProducts[order.Product.Id].Price;

        private void SellProducts(List<ProductOrder> orders)
        {
            foreach (ProductOrder order in orders)
            {
                _availableProducts[order.Product.Id].DecreaseCount(order.Count);
            }
        }
    }
}