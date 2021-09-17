using System;
using Shops.Tools;

namespace Shops.Models
{
    public class ProductOrder
    {
        public ProductOrder(Product? product, int count)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            if (count <= 0)
                throw new ShopsException($"Invalid order count - {count}");
            Product = product;
            Count = count;
        }

        public Product Product { get; init; }
        public int Count { get; init; }
    }
}