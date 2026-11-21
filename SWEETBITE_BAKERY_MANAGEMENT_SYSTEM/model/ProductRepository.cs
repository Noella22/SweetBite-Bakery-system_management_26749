
using System;
using System.Data;
using MySqlConnector;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public class ProductRepository
    {
        public DataTable GetAll()
        {
            return Search("");
        }

        public DataTable Search(string keyword)
        {
            string sql = @"SELECT product_id, product_name, unit_price
                           FROM products
                           WHERE product_name LIKE @search
                           ORDER BY product_id DESC";

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

        public void Insert(Product product)
        {
            string sql = "INSERT INTO products (product_name, unit_price) VALUES (@name, @price)";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@price", product.UnitPrice);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Product product)
        {
            string sql = "UPDATE products SET product_name=@name, unit_price=@price WHERE product_id=@id";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", product.ProductId);
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@price", product.UnitPrice);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM products WHERE product_id=@id";

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
            using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM products", con))
            {
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
