using System;
using Shops.Models;

namespace Shops.Tools
{
    public class ShopsException : Exception
    {
        public ShopsException()
        {
        }

        public ShopsException(string message)
            : base(message)
        {
        }

        public ShopsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static ShopsException ProductIsNotPresentedAtShop(Product product, Shop shop) =>
            new ShopsException($"Product {product.Name} is not presented at shop {shop.Name}");
    }
}