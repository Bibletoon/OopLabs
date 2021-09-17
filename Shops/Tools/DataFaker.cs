using System.Collections.Generic;
using Shops.Models;
using Shops.Services;

namespace Shops.Tools
{
    public class DataFaker
    {
        private ShopManager _shopManager;
        private UserManager _userManager;

        public DataFaker(ShopManager shopManager, UserManager userManager)
        {
            _shopManager = shopManager;
            _userManager = userManager;
        }

        public void CreateData()
        {
            FakeUserManagerData();
            FakeShopManagerData();
        }

        private void FakeUserManagerData()
        {
            var user1 = new User("Person 1", 1000);
            var user2 = new User("Person 2", 525);
            var user3 = new User("Person 3", 10);
            var user4 = new User("Person 4", 777);

            _userManager.RegisterUser(user1);
            _userManager.RegisterUser(user2);
            _userManager.RegisterUser(user3);
            _userManager.RegisterUser(user4);
        }

        private void FakeShopManagerData()
        {
            var shop1 = new Shop("Shop 1");
            var shop2 = new Shop("Shop 2");
            var shop3 = new Shop("Shop 3");
            _shopManager.RegisterShop(shop1);
            _shopManager.RegisterShop(shop2);
            _shopManager.RegisterShop(shop3);

            var product1 = _shopManager.RegisterProduct("Product 1");
            var product2 = _shopManager.RegisterProduct("Product 2");
            var product3 = _shopManager.RegisterProduct("Product 3");
            var product4 = _shopManager.RegisterProduct("Product 4");
            var product5 = _shopManager.RegisterProduct("Product 5");
            var product6 = _shopManager.RegisterProduct("Product 6");
            var product7 = _shopManager.RegisterProduct("Product 7");
            var product8 = _shopManager.RegisterProduct("Product 8");
            var product9 = _shopManager.RegisterProduct("Product 9");
            var product10 = _shopManager.RegisterProduct("Product 10");
            var product11 = _shopManager.RegisterProduct("Product 11");
            var product12 = _shopManager.RegisterProduct("Product 12");
            var product13 = _shopManager.RegisterProduct("Product 13");
            var product14 = _shopManager.RegisterProduct("Product 14");
            var product15 = _shopManager.RegisterProduct("Product 15");
            var product16 = _shopManager.RegisterProduct("Product 16");
            var product17 = _shopManager.RegisterProduct("Product 17");
            var product18 = _shopManager.RegisterProduct("Product 18");
            var product19 = _shopManager.RegisterProduct("Product 19");

            shop1.AddLots(new List<Lot>()
            {
                product1.ToLot(10, 100),
                product2.ToLot(5, 20),
                product3.ToLot(7, 50),
                product4.ToLot(7, 50),
                product5.ToLot(7, 50),
                product6.ToLot(7, 50),
                product7.ToLot(7, 50),
                product8.ToLot(7, 50),
                product9.ToLot(7, 50),
                product10.ToLot(7, 50),
                product11.ToLot(7, 50),
                product12.ToLot(7, 50),
                product13.ToLot(7, 50),
                product14.ToLot(7, 50),
                product15.ToLot(7, 50),
                product16.ToLot(7, 50),
                product17.ToLot(7, 50),
                product18.ToLot(7, 50),
                product19.ToLot(7, 50),
            });

            shop2.AddLots(new List<Lot>()
            {
                product1.ToLot(1, 30),
                product3.ToLot(500, 20),
                product4.ToLot(700, 50),
            });
        }
    }
}