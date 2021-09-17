using System.Collections.Generic;
using Shops.Models;

namespace Shops.UI.Commands.CommandArguments
{
    public class SupplyProductsCommandArguments
    {
        public SupplyProductsCommandArguments(Shop shop, List<Lot> lots)
        {
            Shop = shop;
            Lots = lots;
        }

        public Shop Shop { get; }
        public List<Lot> Lots { get; }
    }
}