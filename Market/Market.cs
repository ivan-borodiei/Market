using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    //master
    class Product
    {
        public string Name { get; set; }
    }
    //fea
    class ProductPrice
    {
        public Product Product { get; set; }

        public PricePolicy ItemPrice { get; set; }

        public PricePolicy VolumePrice { get; set; }
    }

    class PricePolicy
    {
        public int ItemCount { get; set; }

        public double Price { get; set; }
    }

    interface IPriceListFactory
    {
        List<ProductPrice> GetPriceList();
    }

    class PriceListFactory : IPriceListFactory
    {
        public List<ProductPrice> GetPriceList()
        {
            var priceList = new List<ProductPrice>()
            {
                new ProductPrice
                {
                    Product = new Product() { Name = "A" },
                    ItemPrice = new PricePolicy() { ItemCount = 1, Price = 1.25 },
                    VolumePrice = new PricePolicy() { ItemCount = 3, Price = 3 }
                },
                new ProductPrice
                {
                    Product = new Product() { Name = "B" },
                    ItemPrice = new PricePolicy() { ItemCount = 1, Price = 4.25 }
                },
                new ProductPrice
                {
                    Product = new Product() { Name = "C" },
                    ItemPrice = new PricePolicy() { ItemCount = 1, Price = 1.25 },
                    VolumePrice = new PricePolicy() { ItemCount = 3, Price = 3 }
                },
                new ProductPrice
                {
                    Product = new Product() { Name = "D" },
                    ItemPrice = new PricePolicy() { ItemCount = 1, Price = 1 },
                    VolumePrice = new PricePolicy() { ItemCount = 6, Price = 5 }
                }
            };

            return priceList;
        }
    }

    class Client
    {
        public Client()
        {

        }

        public void Process()
        {
            IPriceListFactory priceListFactory = new PriceListFactory();

            Terminal terminal = new Terminal(priceListFactory);

            terminal.Scan("A");
            terminal.Scan("B");
            terminal.Scan("C");
            terminal.Scan("D");
            terminal.Scan("A");
            terminal.Scan("B");
            terminal.Scan("A");

            var result = terminal.CalculateTotal();

            Console.WriteLine($"General sum: {result}");
        }
    }

    class Terminal
    {
        private List<ProductPrice> priceList;
        private List<Product> productList = new List<Product>();

        public Terminal(IPriceListFactory priceListFactory)
        {
            if (priceListFactory == null)
            {
                throw new ArgumentNullException(nameof(priceListFactory));
            }

            this.priceList = priceListFactory.GetPriceList();
        }

        public void SetPricing(List<ProductPrice> priceList)
        {
            this.priceList = priceList;
        }

        //scan comment
        public void Scan(string productName)
        {
            var price = priceList.FirstOrDefault(p => p.Product?.Name == productName);

            if (price == null)
            {
                throw new Exception("Product price has not been found");
            }

            productList.Add(price.Product);
        }

        public double CalculateTotal()
        {
            double result = 0;

            var groupedProductList = productList
                .GroupBy(g => g)
                .Select(g => new { Product = g.Key, Count = g.Count() });

            foreach (var p in groupedProductList)
            {
                result = result + CalculateProductSum(p.Product.Name, p.Count);
            }

            return result;
        }

        private double CalculateProductSum(string name, int count)
        {
            var productPrice = priceList.FirstOrDefault(p => p.Product?.Name == name);
            if (productPrice != null)
            {
                int calculatedItemCount = 0;

                var res = productPrice.VolumePrice == null ? 0 : GetSumByPolicy(productPrice.VolumePrice, count, out calculatedItemCount);

                int restCount = count - calculatedItemCount;

                res = res + (productPrice.ItemPrice == null ? 0 : GetSumByPolicy(productPrice.ItemPrice, restCount, out calculatedItemCount));

                if (restCount != calculatedItemCount)
                {
                    throw new Exception("Some items has not been calculated!");
                }

                return res;
            }

            return 0;
        }

        private double GetSumByPolicy(PricePolicy policy, int count, out int calculatedItemCount)
        {
            var volumeCount = count / policy.ItemCount;

            var res = volumeCount * policy.Price;

            calculatedItemCount = volumeCount * policy.ItemCount;

            return res;
        }
    }
}
