using System;
using System.Data;
using MySqlConnector;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model;


namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller
{
    public static class OrderController
    {
        public static DataTable GetAvailableProducts()
        {
            OrderRepository repo = new OrderRepository();
            return repo.GetAvailableProducts();
        }

        public static int Save(int customerId, DataTable items)
        {
            if (customerId <= 0 || items == null)
                throw new ArgumentException("Select a customer and add products.");

            int count = 0;

            foreach (DataRow row in items.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;

                if (Convert.ToInt32(row["product_id"]) <= 0 ||
                    Convert.ToInt32(row["quantity"]) <= 0)
                    throw new ArgumentException("Enter valid product quantities.");

                count++;
            }

            if (count == 0)
                throw new ArgumentException("Add at least one product.");

            Order order = new Order();
            order.CustomerId = customerId;
            order.Status = "Pending";

            OrderRepository repo = new OrderRepository();
            return repo.InsertOrder(order, items);
        }

        public static DataTable GetHistory(string search)
        {
            OrderRepository repo = new OrderRepository();
            return repo.GetHistory(search ?? "");
        }

        public static DataTable GetItems(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Select an order.");

            OrderRepository repo = new OrderRepository();
            return repo.GetItems(orderId);
        }
    }
}
