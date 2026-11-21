using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySqlConnector;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public class InventoryRepository
    {
        // Display product stock
        public DataTable GetStock()
        {
            string sql = @"SELECT p.product_id,
                           p.product_name,
                           'Not stored' AS category,
                           p.unit_price,
                           COALESCE(i.quantity_in_stock, 0) AS stock_qty,
                           CASE WHEN COALESCE(i.quantity_in_stock, 0) > 0
                                THEN 1 ELSE 0 END AS is_available
                           FROM products p
                           LEFT JOIN inventory i
                           ON p.product_id = i.product_id
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

        // Save or update stock
        public void Save(InventoryItem item)
        {
            string sql = @"INSERT INTO inventory
                           (product_id, quantity_in_stock)
                           VALUES (@id, @quantity)
                           ON DUPLICATE KEY UPDATE
                           quantity_in_stock = @quantity";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))

            using (MySqlCommand cmd = new MySqlCommand(sql, con))
                
            {
                cmd.Parameters.AddWithValue("@id", item.ProductId);
                cmd.Parameters.AddWithValue("@quantity",item.QuantityInStock);
                    

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
