using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using NUnit.Framework;
using Shops.Models;
using Shops.Services;
using Shops.Tools;
using Shops.UI.Commands;
using Shops.UI.Commands.CommandArguments;

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
        [TestCase(10, 20)]
        public void AddNewLots_LotsWasAddedWithProperPriceAndCount(int productsCount, int productsPrice)
        {
            Shop shop = new Shop("Shop");
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
        [TestCase(10,10,20,20)]
        public void AddMoreLots_CountIncreasedAndPriceSaved(int startCount, int price, int addCount, int addPrice)
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            
            shop.AddLot(product.ToLot(startCount,price));
            shop.AddLot(product.ToLot(addCount,addPrice));

            Lot info = shop.GetProductInfo(product);
            
            Assert.AreEqual(startCount+addCount, info.Count);
            Assert.AreEqual(price, info.Price);
        }

        [Test]
        [TestCase(10, 10)]
        public void SellAllProductLots_ProductIsNotAvailable(int productsCount, int productPrice)
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            User user = new User("User", productsCount * productPrice);
            
            shop.AddLot(product.ToLot(productsCount,productPrice));
            shop.Buy(user, product.ToOrder(productsCount));

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.GetProductInfo(product);
                                         });
        }

        [Test]
        [TestCase(3, 20)]
        public void AddLotsAfterSellingAll_LotsAvailableWithNewPrice(int addProductsCount, int addProductsPrice)
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("product");
            User user = new User("User", 100);
            shop.AddLot(product.ToLot(1,1));
            
            shop.Buy(user,new List<ProductOrder>(){product.ToOrder(1)});
            
            shop.AddLot(product.ToLot(addProductsCount,addProductsPrice));

            Lot info = shop.GetProductInfo(product);
            Assert.AreEqual(addProductsCount,info.Count);
            Assert.AreEqual(addProductsPrice,info.Price);
        }

        [Test]
        [TestCase(10,50,100,20)]
        public void AddLotsToTwoShops_LotsAreIndependent(int lotsCount, int lotsStartPrice, int firstLotNewPrice, int secondLotNewPrice)
        {
            Shop shop1 = new Shop("Shop 1");
            Shop shop2 = new Shop("Shopp 2");
            Product product = _shopManager.RegisterProduct("product");
            Lot lot = product.ToLot(lotsCount, lotsStartPrice);
            
            shop1.AddLot(lot);
            shop2.AddLot(lot);

            shop1.SetProductPrice(product, firstLotNewPrice);
            shop2.SetProductPrice(product, secondLotNewPrice);
            
            Assert.AreEqual(firstLotNewPrice, shop1.GetProductInfo(product).Price);
            Assert.AreEqual(secondLotNewPrice, shop2.GetProductInfo(product).Price);
        }
        
        [Test]
        public void SupplyProductsWithTooBigCount_ThrowsException()
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            
            shop.AddLot(product.ToLot(1000, 100));

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.AddLot(product.ToLot(int.MaxValue, 100));
                                         });
            
            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.AddLots(new List<Lot>(){product.ToLot(int.MaxValue, 100)});
                                         });
        }

        #endregion

        #region ShopProductsTests

        [Test]
        [TestCase(100, 20)]
        public void ChangeProductPrice_ProductAvailableWithNewPrice(int startPrice, int newPrice)
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            
            shop.AddLot(product.ToLot(10,startPrice));

            shop.SetProductPrice(product, newPrice);
            
            Lot info = shop.GetProductInfo(product);
            
            Assert.AreEqual(newPrice, info.Price);
        }
        
        [Test]
        public void ChangePriceForNotAvailableProduct_ThrowsException()
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.SetProductPrice(product, 100);
                                         });
        }

        [Test]
        public void ChangePriceToInvalid_ThrowsException()
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            shop.AddLot(product.ToLot(10,10));
            
            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.SetProductPrice(product, -12);
                                         });
        }

        [Test]
        [TestCase(10, 100)]
        public void GetInfoForProduct_ReturnsProperInfo(int productCount, int productPrice)
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            shop.AddLot(product.ToLot(productCount,productPrice));
            var productInfo = shop.GetProductInfo(product);
            Assert.AreEqual(productInfo.Price, productPrice);
            Assert.AreEqual(productInfo.Count, productCount);
        }

        [Test]
        public void GetInfoForNotAvailableProduct_ThrowsException()
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.GetProductInfo(product);
                                         });
        }

        [Test]
        public void GetAllProducts_ShouldReturnProperProducts()
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            Product product2 = _shopManager.RegisterProduct("Product 2");
            var products = _shopManager.GetAllProducts();
            Assert.Contains(product, products.ToList());
            Assert.Contains(product2, products.ToList());
        }

        #endregion
        
        #region PurchasesTests

        [Test]
        [TestCase(100, 30, 5, 3)]
        public void BuyProduct_MoneyAndProductsCountChangedProperly(
            int moneyBefore, 
            int productPrice,
            int productCount,
            int productToBuyCount)
        {
            var person = new User("name", moneyBefore);
            var shopManager = new ShopManager();
            var shop = new Shop("shop name");
            var product = shopManager.RegisterProduct("product name");

            var lot = product.ToLot(productCount, productPrice);
            
            shop.AddLot(lot);

            var toBuy = new ProductOrder(product, productToBuyCount);
            
            shop.Buy(person, new List<ProductOrder>(){toBuy});
            
            Assert.AreEqual(moneyBefore - productPrice  * productToBuyCount, person.Money);
            Assert.AreEqual(productCount - productToBuyCount , shop.GetProductInfo(product).Count);
        }
        
        [Test]
        [TestCase(1000)]
        public void BuyMoreProductsThanAvailable_ThrowsExceptionMoneyDontChange(int personMoney)
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            User user = new User("User",personMoney);
            
            shop.AddLot(product.ToLot(1,1));

            Assert.Catch<ShopsException>(() =>
                                          {
                                              shop.Buy(user, new List<ProductOrder>()
                                              {
                                                  product.ToOrder(10)
                                              });
                                          });
            
            Assert.AreEqual(personMoney,user.Money);
        }

        [Test]
        [TestCase(100)]
        public void BuyProductsWithNotEnoughMoney_ThrowsExceptionProductCountDontChange(int productsCount)
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            User user = new User("User",1);
            
            shop.AddLot(product.ToLot(productsCount,100));

            Assert.Catch<ShopsException>(() =>
                                          {
                                              shop.Buy(user, new List<ProductOrder>()
                                              {
                                                  product.ToOrder(10)
                                              });
                                          });

            Lot info = shop.GetProductInfo(product);
            
            Assert.AreEqual(productsCount,info.Count);
        }

        [Test]
        public void CreateOrderWithSameProducts_ThrowsException()
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            User user = new User("User", 1000);
            
            shop.AddLot(product.ToLot(100,100));

            List<ProductOrder> orders = new List<ProductOrder>()
            {
                product.ToOrder(10),
                product.ToOrder(20)
            };

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.Buy(user, orders);
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
            Shop shop1 = new Shop("Shop 1");
            Shop shop2 = new Shop("Shop 2");
            Shop shop3 = new Shop("Shop 3");
            Shop shop4 = new Shop("Shop 4");

            _shopManager.RegisterShop(shop1);
            _shopManager.RegisterShop(shop2);
            _shopManager.RegisterShop(shop3);
            _shopManager.RegisterShop(shop4);
            
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
            Shop shop1 = new Shop("Shop 1");
            Shop shop2 = new Shop("Shop 2");
            Shop shop3 = new Shop("Shop 3");
            Shop shop4 = new Shop("Shop 4");
            Product product = _shopManager.RegisterProduct("Product");
            
            _shopManager.RegisterShop(shop1);
            _shopManager.RegisterShop(shop2);
            _shopManager.RegisterShop(shop3);
            _shopManager.RegisterShop(shop4);

            Shop? bestShop = _shopManager.FindShopWithBestOffer(product.ToOrder(5));
            Assert.IsNull(bestShop);
        }

        [Test]
        public void FindMinimalPriceForNotEnoughOrder_ReturnNull()
        {
            Shop shop1 = new Shop("Shop 1");
            Shop shop2 = new Shop("Shop 2");
            Shop shop3 = new Shop("Shop 3");
            Shop shop4 = new Shop("Shop 4");
            Product product = _shopManager.RegisterProduct("Product");
            
            _shopManager.RegisterShop(shop1);
            _shopManager.RegisterShop(shop2);
            _shopManager.RegisterShop(shop3);
            _shopManager.RegisterShop(shop4);

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

            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new Shop(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new User(null, 100);
                                                });
        }

        [Test]
        public void ProvideNullArgumentsToShopMethods_ThrowsException()
        {
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            User user = new User("User",100);
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
            Shop shop = new Shop("Shop");
            Product product = _shopManager.RegisterProduct("Product");
            User user = new User("User", 1000);
            
            shop.AddLot(product.ToLot(100,100));

            List<ProductOrder> orders = new List<ProductOrder>()
            {
                product.ToOrder(10),
                null,
                null
            };

            Assert.Catch<ShopsException>(() =>
                                         {
                                             shop.Buy(user, orders);
                                         });
        }
        
        [Test]
        public void AddNullLots_ThrowsException()
        {
            var shopManager = new ShopManager();
            Shop shop = new Shop("shop name");
            Product product = shopManager.RegisterProduct("product name");

            Assert.Catch<ShopsException>(() =>
                                             shop.AddLots(new List<Lot>() { product.ToLot(10, 10), null, null })
            );
        }
        
        #endregion
    }
}