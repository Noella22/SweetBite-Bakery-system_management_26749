using System;
using System.Data;
using MySqlConnector;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller
{
    public static class InventoryController
    {
        public static DataTable GetStock()
        {
            InventoryRepository repo = new InventoryRepository();
            return repo.GetStock();
        }

        public static void Save(int id, string category, int quantity, bool available)
            
        {
            if (id <= 0 || quantity < 0)
            {
                throw new ArgumentException(
                    "Select a product and enter a valid quantity.");
            }

            InventoryItem item = new InventoryItem();
            item.ProductId = id;
            item.QuantityInStock = quantity;

            InventoryRepository repo = new InventoryRepository();
            repo.Save(item);
        }
    }
}
