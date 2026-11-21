using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Data;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public class SaleRepository
    {
        
        public DataTable Search(string keyword)
        {
            string sql = @"SELECT sale_id, product_id, product_name,
                           quantity, unit_price, total_amount, sale_date
                           FROM sales
                           WHERE product_name LIKE @search
                           ORDER BY sale_id DESC";

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

        
        public Product GetProduct(int id)
        {
            string sql = "SELECT product_id, product_name, unit_price FROM products WHERE product_id=@id";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new ArgumentException("Select a valid product.");

                    Product product = new Product();
                    product.ProductId = Convert.ToInt32(reader["product_id"]);
                    product.ProductName = reader["product_name"].ToString();
                    product.UnitPrice = Convert.ToDecimal(reader["unit_price"]);

                    return product;
                }
            }
        }

        
        public void Insert(Sale sale)
        {
            string sql = @"INSERT INTO sales
                           (product_id, product_name, quantity,
                            unit_price, total_amount, sale_date)
                           VALUES (@pid, @name, @qty, @price, @total, NOW())";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@pid", sale.ProductId);
                cmd.Parameters.AddWithValue("@name", sale.ProductName);
                cmd.Parameters.AddWithValue("@qty", sale.Quantity);
                cmd.Parameters.AddWithValue("@price", sale.UnitPrice);
                cmd.Parameters.AddWithValue("@total", sale.TotalAmount);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        
        public void Update(Sale sale)
        {
            string sql = @"UPDATE sales SET
                           product_id=@pid, product_name=@name,
                           quantity=@qty, unit_price=@price,
                           total_amount=@total
                           WHERE sale_id=@id";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", sale.SaleId);
                cmd.Parameters.AddWithValue("@pid", sale.ProductId);
                cmd.Parameters.AddWithValue("@name", sale.ProductName);
                cmd.Parameters.AddWithValue("@qty", sale.Quantity);
                cmd.Parameters.AddWithValue("@price", sale.UnitPrice);
                cmd.Parameters.AddWithValue("@total", sale.TotalAmount);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

       
        public void Delete(int id)
        {
            string sql = "DELETE FROM sales WHERE sale_id=@id";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        
        public int Count()
        {
            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM sales", con))
            {
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        
        public decimal TotalRevenue()
        {
            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT COALESCE(SUM(total_amount), 0) FROM sales", con))
            {
                con.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }
    }
}
