using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySqlConnector;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public class OrderRepository
    {
        // Products that have stock
        public DataTable GetAvailableProducts()
        {
            string sql = @"SELECT p.product_id, p.product_name,
                           p.unit_price,
                           i.quantity_in_stock AS stock_qty
                           FROM products p
                           JOIN inventory i ON p.product_id = i.product_id
                           WHERE i.quantity_in_stock > 0
                           ORDER BY p.product_name";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        // Save order, order items and update stock
        public int InsertOrder(Order order, DataTable items)
        {
            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            {
                con.Open();

                using (MySqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        string sql = @"INSERT INTO orders
                                       (customer_id, order_date, status)
                                       VALUES (@customer, NOW(), @status)";

                        int orderId;

                        using (MySqlCommand cmd = new MySqlCommand(sql, con, tx))
                        {
                            cmd.Parameters.AddWithValue("@customer", order.CustomerId);
                            cmd.Parameters.AddWithValue("@status", order.Status);
                            cmd.ExecuteNonQuery();
                            orderId = Convert.ToInt32(cmd.LastInsertedId);
                        }

                        foreach (DataRow row in items.Rows)
                        {
                            if (row.RowState == DataRowState.Deleted)
                                continue;

                            int productId = Convert.ToInt32(row["product_id"]);
                            int quantity = Convert.ToInt32(row["quantity"]);
                            decimal price;

                            // Get the current product price
                            using (MySqlCommand cmd = new MySqlCommand(
                                "SELECT unit_price FROM products WHERE product_id=@id",
                                con, tx))
                            {
                                cmd.Parameters.AddWithValue("@id", productId);
                                object result = cmd.ExecuteScalar();

                                if (result == null)
                                    throw new Exception("Product not found.");

                                price = Convert.ToDecimal(result);
                            }

                            // Reduce stock only when enough is available
                            using (MySqlCommand cmd = new MySqlCommand(
                                @"UPDATE inventory
                                  SET quantity_in_stock=quantity_in_stock-@qty
                                  WHERE product_id=@id
                                  AND quantity_in_stock>=@qty", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@id", productId);
                                cmd.Parameters.AddWithValue("@qty", quantity);

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception("Not enough stock.");
                            }

                            OrderItem item = new OrderItem();
                            item.OrderId = orderId;
                            item.ProductId = productId;
                            item.Quantity = quantity;
                            item.UnitPrice = price;
                            item.TotalAmount = price * quantity;

                            // Save the ordered product
                            using (MySqlCommand cmd = new MySqlCommand(
                                @"INSERT INTO order_items
                                  (order_id, product_id, quantity,
                                   unit_price, total_amount)
                                  VALUES (@order, @product, @qty,
                                          @price, @total)", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@order", item.OrderId);
                                cmd.Parameters.AddWithValue("@product", item.ProductId);
                                cmd.Parameters.AddWithValue("@qty", item.Quantity);
                                cmd.Parameters.AddWithValue("@price", item.UnitPrice);
                                cmd.Parameters.AddWithValue("@total", item.TotalAmount);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        return orderId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        // Display order history
        public DataTable GetHistory(string keyword)
        {
            string sql = @"SELECT o.order_id, c.customer_name,
                           o.order_date, o.status,
                           COALESCE(SUM(oi.total_amount), 0) AS total_amount
                           FROM orders o
                           JOIN customers c ON o.customer_id=c.customer_id
                           LEFT JOIN order_items oi ON o.order_id=oi.order_id
                           WHERE CAST(o.order_id AS CHAR) LIKE @search
                           OR c.customer_name LIKE @search
                           GROUP BY o.order_id, c.customer_name,
                                    o.order_date, o.status
                           ORDER BY o.order_id DESC";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@search", "%" + keyword + "%");
                con.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        // Products belonging to one order
        public DataTable GetItems(int orderId)
        {
            string sql = @"SELECT p.product_name, oi.quantity,
                           oi.unit_price,
                           oi.total_amount AS line_total
                           FROM order_items oi
                           JOIN products p ON oi.product_id=p.product_id
                           WHERE oi.order_id=@id";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", orderId);
                con.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }
    }
}