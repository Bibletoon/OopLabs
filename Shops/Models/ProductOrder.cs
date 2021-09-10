using System;
using Shops.Tools;

namespace Shops.Models
{
    public class ProductOrder
    {
        public ProductOrder(Product? product, int count)
        {
            if (count <= 0)
                throw new ShopsException($"Invalid order count - {count}");
            Product = product.ThrowIfNull(new ArgumentNullException(nameof(product)));
            Count = count;
        }

        public Product Product { get; init; }
        public int Count { get; init; }
    }
}