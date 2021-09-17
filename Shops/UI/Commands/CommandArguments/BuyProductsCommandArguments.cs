using System.Collections.Generic;
using Shops.Models;

namespace Shops.UI.Commands.CommandArguments
{
    public class BuyProductsCommandArguments
    {
        public BuyProductsCommandArguments(User user, Shop shop, List<ProductOrder> orders)
        {
            User = user;
            Shop = shop;
            Orders = orders;
        }

        public User User { get; }
        public Shop Shop { get; }
        public List<ProductOrder> Orders { get; }
    }
}