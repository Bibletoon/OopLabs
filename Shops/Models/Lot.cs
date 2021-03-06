using System;
using Shops.Tools;

namespace Shops.Models
{
    public class Lot
    {
        public Lot(Product product, int count, int price)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            if (count <= 0)
                throw new ShopsException($"Invalid lot count - {count}");

            if (price <= 0)
                throw new ShopsException($"Invalid lot price - {price}");

            Product = product;
            Count = count;
            Price = price;
        }

        public Product Product { get; init; }
        public int Count { get; private set; }
        public int Price { get; private set; }

        internal bool CanIncreaseCount(int count)
        {
            long sum = (long)Count + count;
            int intSum = unchecked(Count + count);
            return sum == intSum;
        }

        internal void IncreaseCount(int count)
        {
            Count += count;
        }

        internal void DecreaseCount(int count)
        {
            if (count > Count)
                throw new ShopsException("Products count can't be less than zero");

            Count -= count;
        }

        internal void SetPrice(int price)
        {
            Price = price;
        }
    }
}