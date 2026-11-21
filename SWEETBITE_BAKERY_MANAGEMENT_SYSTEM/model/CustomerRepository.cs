using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySqlConnector;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public class CustomerRepository
    {
        // Display and search customers
        public DataTable Search(string keyword)
        {
            string sql = @"SELECT customer_id, customer_name,
                           phone, email FROM customers
                           WHERE customer_name LIKE @search
                           OR phone LIKE @search
                           OR email LIKE @search
                           ORDER BY customer_id DESC";

            using (MySqlConnection con =
                new MySqlConnection(Database.ConnectionString))
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

        // Display all customers
        public DataTable GetAll()
        {
            return Search("");
        }

        // Add customer
        public void Insert(Customer customer)
        {
            string sql = @"INSERT INTO customers
                           (customer_name, phone, email)
                           VALUES (@name, @phone, @email)";

            using (MySqlConnection con =
                new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@name", customer.CustomerName);
                cmd.Parameters.AddWithValue("@phone", customer.Phone);
                cmd.Parameters.AddWithValue("@email", customer.Email);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Update customer
        public void Update(Customer customer)
        {
            string sql = @"UPDATE customers
                         SET customer_name=@name, phone=@phone
                         WHERE customer_id=@id";

            using (MySqlConnection con =
                new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", customer.CustomerId);
                cmd.Parameters.AddWithValue("@name", customer.CustomerName);
                cmd.Parameters.AddWithValue("@phone", customer.Phone);
               

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete customer
        public void Delete(int id)
        {
            string sql = "DELETE FROM customers WHERE customer_id=@id";

            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
                
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
