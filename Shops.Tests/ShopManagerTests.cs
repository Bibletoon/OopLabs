using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using NUnit.Framework;
using Shops.Models;
using Shops.Services;
using Shops.Tools;

namespace Shops.Tests
{
    public class ShopManagerTests
    {
        private ShopManager _shopManager;
        
        [SetUp]
        public void Init()
        {
            _shopManager = new ShopManager();
        }

        #region SupplyTests
        
        [Test]
        public void AddNewLots_LotsWasAddedWithProperPriceAndCount()
        {
            const int productsCount = 10;
            const int productsPrice = 20;
            
            Shop shop = _shopManager.Create("Shop");
            Product product1 = _shopManager.RegisterProduct("Product 1");
            Product product2 = _shopManager.RegisterProduct("Product 2");
            Product product3 = _shopManager.RegisterProduct("Product 3");

            
            shop.AddLot(product1.ToLot(productsCount,productsPrice));

            List<Lot> lots = new List<Lot>()
            {
                product2.ToLot(productsCount, productsPrice),
                product3.ToLot(productsCount, productsPrice)
            };
            
            shop.AddLots(lots);

            Lot info1 = shop.GetProductInfo(product1);
            Lot info2 = shop.GetProductInfo(product2);
            Lot info3 = shop.GetProductInfo(product3);
            
            Assert.AreEqual(productsCount, info1.Count);
            Assert.AreEqual(productsCount, info2.Count);
            Assert.AreEqual(productsCount, info3.Count);
            Assert.AreEqual(productsPrice, info1.Price);
            Assert.AreEqual(productsPrice, info2.Price);
            Assert.AreEqual(productsPrice, info3.Price);
        }

        [Test]
        public void CreateLotsWithInvalidPriceOrCount_ThrowsException()
        {
            Product product = _shopManager.RegisterProduct("Product");

            Assert.Catch<ShopsException>(() =>
                                         {
                                             new Lot(product, 0, 10);
                                         });
            
            Assert.Catch<ShopsException>(() =>
                                         {
                                             new Lot(product, 10, 0);
                                         });
        }

        [Test]
        public void AddMoreLots_CountIncreasedAndPriceSaved()
        {
            const int startCount = 10;
            const int price = 10;
            const int addCount = 20;
            const int addPrice = 20;
            
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            
            shop.AddLot(product.ToLot(startCount,price));
            shop.AddLot(product.ToLot(addCount,addPrice));

            Lot info = shop.GetProductInfo(product);
            
            Assert.AreEqual(startCount+addCount, info.Count);
            Assert.AreEqual(price, info.Price);
        }

        [Test]
        public void SellAllProductLots_ProductIsNotAvailable()
        {
            const int productsCount = 10;
            const int productPrice = 10;
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            Person person = new Person("Person", productsCount * productPrice);
            
            shop.AddLot(product.ToLot(productsCount,productPrice));
            shop.Buy(person, product.ToOrder(productsCount));

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.GetProductInfo(product);
                                         });
        }

        [Test]
        public void AddLotsAfterSellingAll_LotsAvailableWithNewPrice()
        {
            const int addProductsCount = 3;
            const int addProductsPrice = 20;
            
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("product");
            Person person = new Person("Person", 100);
            shop.AddLot(product.ToLot(1,1));
            
            shop.Buy(person,new List<ProductOrder>(){product.ToOrder(1)});
            
            shop.AddLot(product.ToLot(addProductsCount,addProductsPrice));

            Lot info = shop.GetProductInfo(product);
            Assert.AreEqual(addProductsCount,info.Count);
            Assert.AreEqual(addProductsPrice,info.Price);
        }

        [Test]
        public void AddLotsToTwoShops_LotsAreIndependent()
        {
            const int lotsStartPrice = 50;
            const int firstLotNewPrice = 100;
            const int secondLotNewPrice = 20;
            
            Shop shop1 = _shopManager.Create("Shop 1");
            Shop shop2 = _shopManager.Create("Shopp 2");
            Product product = _shopManager.RegisterProduct("product");
            Lot lot = product.ToLot(10, 50);
            
            shop1.AddLot(lot);
            shop2.AddLot(lot);

            shop1.SetProductPrice(product, firstLotNewPrice);
            shop2.SetProductPrice(product, secondLotNewPrice);
            
            Assert.AreEqual(firstLotNewPrice, shop1.GetProductInfo(product).Price);
            Assert.AreEqual(secondLotNewPrice, shop2.GetProductInfo(product).Price);
        }

        #endregion

        #region ShopProductsTests

        [Test]
        public void ChangeProductPrice_ProductAvailableWithNewPrice()
        {
            const int startPrice = 100;
            const int newPrice = 20;
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            
            shop.AddLot(product.ToLot(10,startPrice));

            shop.SetProductPrice(product, newPrice);
            
            Lot info = shop.GetProductInfo(product);
            
            Assert.AreEqual(newPrice, info.Price);
        }
        
        [Test]
        public void ChangePriceForNotAvailableProduct_ThrowsException()
        {
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.SetProductPrice(product, 100);
                                         });
        }

        [Test]
        public void ChangePriceToInvalid_ThrowsException()
        {
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            shop.AddLot(product.ToLot(10,10));
            
            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.SetProductPrice(product, -12);
                                         });
        }

        [Test]
        public void GetInfoForNotAvailableProduct_ThrowsException()
        {
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.GetProductInfo(product);
                                         });
        }

        #endregion
        
        #region PurchasesTests

        [Test]
        public void BuyProduct_MoneyAndProductsCountChangedProperly()
        {
            const int moneyBefore = 100;
            const int productPrice = 30;
            const int productCount = 5;
            const int productToBuyCount = 3;

            var person = new Person("name", moneyBefore);
            var shopManager = new ShopManager();
            var shop = shopManager.Create("shop name");
            var product = shopManager.RegisterProduct("product name");

            var lot = product.ToLot(productCount, productPrice);
            
            shop.AddLot(lot);

            var toBuy = new ProductOrder(product, productToBuyCount);
            
            shop.Buy(person, new List<ProductOrder>(){toBuy});
            
            Assert.AreEqual(moneyBefore - productPrice  * productToBuyCount, person.Money);
            Assert.AreEqual(productCount - productToBuyCount , shop.GetProductInfo(product).Count);
        }
        
        [Test]
        public void BuyMoreProductsThanAvailable_ThrowsExceptionMoneyDontChange()
        {
            const int personMoney = 1000;
            
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            Person person = new Person("Person",personMoney);
            
            shop.AddLot(product.ToLot(1,1));

            Assert.Catch<ShopsException>(() =>
                                          {
                                              shop.Buy(person, new List<ProductOrder>()
                                              {
                                                  product.ToOrder(10)
                                              });
                                          });
            
            Assert.AreEqual(personMoney,person.Money);
        }

        [Test]
        public void BuyProductsWithNotEnoughMoney_ThrowsExceptionProductCountDontChange()
        {
            const int productCount = 100;
            
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            Person person = new Person("Person",1);
            
            shop.AddLot(product.ToLot(productCount,100));

            Assert.Catch<ShopsException>(() =>
                                          {
                                              shop.Buy(person, new List<ProductOrder>()
                                              {
                                                  product.ToOrder(10)
                                              });
                                          });

            Lot info = shop.GetProductInfo(product);
            
            Assert.AreEqual(productCount,info.Count);
        }

        [Test]
        public void CreateOrderWithSameProducts_ThrowsException()
        {
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            Person person = new Person("Person", 1000);
            
            shop.AddLot(product.ToLot(100,100));

            List<ProductOrder> orders = new List<ProductOrder>()
            {
                product.ToOrder(10),
                product.ToOrder(20)
            };

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.Buy(person, orders);
                                         });
        }

        [Test]
        public void CreateOrderWithInvalidCount_ThrowsException()
        {
            Product product = _shopManager.RegisterProduct("Product");

            Assert.Catch<ShopsException>(() =>
                                         {
                                             new ProductOrder(product, -10);
                                         });
            
            Assert.Catch<ShopsException>(() =>
                                         {
                                             product.ToOrder(0);
                                         });
        }

        [Test]
        public void FindMinimalPriceForOrder_ReturnProperShop()
        {
            Shop shop1 = _shopManager.Create("Shop 1");
            Shop shop2 = _shopManager.Create("Shop 2");
            Shop shop3 = _shopManager.Create("Shop 3");
            Shop shop4 = _shopManager.Create("Shop 4");
            Product product = _shopManager.RegisterProduct("Product");

            shop1.AddLot(product.ToLot(10,10));
            shop2.AddLot(product.ToLot(7,100));
            shop3.AddLot(product.ToLot(5,5));
            shop4.AddLot(product.ToLot(3,2));

            Shop bestShop = _shopManager.FindShopWithBestOffer(product.ToOrder(5));
            
            Assert.AreEqual(shop3,bestShop);
        }

        [Test]
        public void FindMinimalPriceForNotAvailableOrder_ReturnNull()
        {
            Shop shop1 = _shopManager.Create("Shop 1");
            Shop shop2 = _shopManager.Create("Shop 2");
            Shop shop3 = _shopManager.Create("Shop 3");
            Shop shop4 = _shopManager.Create("Shop 4");
            Product product = _shopManager.RegisterProduct("Product");

            Shop? bestShop = _shopManager.FindShopWithBestOffer(product.ToOrder(5));
            Assert.IsNull(bestShop);
        }

        [Test]
        public void FindMinimalPriceForNotEnoughOrder_ReturnNull()
        {
            Shop shop1 = _shopManager.Create("Shop 1");
            Shop shop2 = _shopManager.Create("Shop 2");
            Shop shop3 = _shopManager.Create("Shop 3");
            Shop shop4 = _shopManager.Create("Shop 4");
            Product product = _shopManager.RegisterProduct("Product");

            shop1.AddLot(product.ToLot(10,10));
            shop2.AddLot(product.ToLot(7,100));
            shop3.AddLot(product.ToLot(5,5));
            shop4.AddLot(product.ToLot(3,2));

            Shop? bestShop = _shopManager.FindShopWithBestOffer(product.ToOrder(50));
            
            Assert.IsNull(bestShop);
        }
        
        #endregion
        
        #region NullabilityTests

        [Test]
        public void ProvideNullArgumentsOnCreate_ThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new Lot(null, 100, 100);
                                                });
            
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new ProductOrder(null, 100);
                                                });
        }

        [Test]
        public void ProvideNullArgumentsToShopMethods_ThrowsException()
        {
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            Person person = new Person("Person",100);
            shop.AddLot(product.ToLot(10,10));
            
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    shop.AddLot(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    shop.AddLots(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    shop.GetProductInfo(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    shop.SetProductPrice(null,100);
                                                });

            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    shop.Buy(null, product.ToOrder(10));
                                                });
        }
        
        [Test]
        public void BuyNullOrders_ThrowsException()
        {
            Shop shop = _shopManager.Create("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            Person person = new Person("Person", 1000);
            
            shop.AddLot(product.ToLot(100,100));

            List<ProductOrder> orders = new List<ProductOrder>()
            {
                product.ToOrder(10),
                null,
                null
            };

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.Buy(person, orders);
                                         });
        }
        
        [Test]
        public void AddNullLots_ThrowsException()
        {
            var shopManager = new ShopManager();
            Shop shop = shopManager.Create("shop name");
            Product product = shopManager.RegisterProduct("product name");

            Assert.Catch<ShopsException>(() =>
                                             shop.AddLots(new List<Lot>() { product.ToLot(10, 10), null, null })
            );
        }
        
        #endregion
    }
}