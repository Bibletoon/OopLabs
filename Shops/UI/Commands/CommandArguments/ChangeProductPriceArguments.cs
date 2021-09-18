using Shops.Models;

namespace Shops.UI.Commands.CommandArguments
{
    public class ChangeProductPriceArguments
    {
        public ChangeProductPriceArguments(Shop shop, Product product, int newPrice)
        {
            Shop = shop;
            Product = product;
            NewPrice = newPrice;
        }

        public Shop Shop { get; }
        public Product Product { get; }
        public int NewPrice { get; }
    }
}