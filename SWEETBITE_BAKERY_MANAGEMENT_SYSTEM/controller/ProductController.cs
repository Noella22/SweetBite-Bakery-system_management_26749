
using System;
using System.Data;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller
{
    public static class ProductController
    {
        public static DataTable GetProducts(string search)
        {
            ProductRepository repo = new ProductRepository();
            return repo.Search(search ?? "");
        }

        public static void Save(string name, decimal price)
        {
            Validate(name, price);

            Product product = new Product();
            product.ProductName = name;
            product.UnitPrice = price;

            ProductRepository repo = new ProductRepository();
            repo.Insert(product);
        }

        public static void Update(int id, string name, decimal price)
        {
            if (id <= 0)
                throw new ArgumentException("Select a product.");

            Validate(name, price);

            Product product = new Product();
            product.ProductId = id;
            product.ProductName = name;
            product.UnitPrice = price;

            ProductRepository repo = new ProductRepository();
            repo.Update(product);
        }

        public static void Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Select a product.");

            ProductRepository repo = new ProductRepository();
            repo.Delete(id);
        }

        public static int Count()
        {
            ProductRepository repo = new ProductRepository();
            return repo.Count();
        }

        private static void Validate(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name) || price <= 0)
                throw new ArgumentException("Enter valid product details.");
        }
    }
}

