using System;
using System.Data;
using MySqlConnector;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller
{
    public static class SaleController
    {
        public static DataTable GetSales(string search)
        {
            SaleRepository repo = new SaleRepository();
            return repo.Search(search ?? "");
        }

        public static void Save(int productId, int quantity)
        {
            SaleRepository repo = new SaleRepository();
            Sale sale = CreateSale(repo, productId, quantity);
            repo.Insert(sale);
        }

        public static void Update(int saleId, int productId, int quantity)
        {
            if (saleId <= 0)
                throw new ArgumentException("Select a sale first.");

            SaleRepository repo = new SaleRepository();
            Sale sale = CreateSale(repo, productId, quantity);
            sale.SaleId = saleId;

            repo.Update(sale);
        }

        public static void Delete(int saleId)
        {
            if (saleId <= 0)
                throw new ArgumentException("Select a sale first.");

            SaleRepository repo = new SaleRepository();
            repo.Delete(saleId);
        }

        public static int Count()
        {
            SaleRepository repo = new SaleRepository();
            return repo.Count();
        }

        public static decimal TotalRevenue()
        {
            SaleRepository repo = new SaleRepository();
            return repo.TotalRevenue();
        }

        private static Sale CreateSale(
            SaleRepository repo, int productId, int quantity)
        {
            if (productId <= 0 || quantity <= 0)
                throw new ArgumentException("Select a product and enter a valid quantity.");

            Product product = repo.GetProduct(productId);

            Sale sale = new Sale();
            sale.ProductId = product.ProductId;
            sale.ProductName = product.ProductName;
            sale.Quantity = quantity;
            sale.UnitPrice = product.UnitPrice;
            sale.TotalAmount = product.UnitPrice * quantity;

            return sale;
        }
    }
}