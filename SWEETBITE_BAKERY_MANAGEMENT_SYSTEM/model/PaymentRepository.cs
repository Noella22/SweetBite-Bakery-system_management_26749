using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySqlConnector;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public class PaymentRepository
    {
        // Display payments
        public DataTable GetAll()
        {
            string sql = @"SELECT payment_id, order_id,
                           payment_method, amount, payment_date
                           FROM payments ORDER BY payment_id DESC";

            using (MySqlConnection con =
                new MySqlConnection(Database.ConnectionString))
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

        // Record payment
        public void Insert(Payment payment)
        {
            using (MySqlConnection con =
                new MySqlConnection(Database.ConnectionString))
            {
                con.Open();

                using (MySqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        // Check and lock the order
                        using (MySqlCommand cmd = new MySqlCommand(
                            "SELECT order_id FROM orders WHERE order_id=@id FOR UPDATE",
                            con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", payment.OrderId);

                            if (cmd.ExecuteScalar() == null)
                                throw new Exception("Order not found.");
                        }

                        // Calculate order total
                        decimal total;
                        using (MySqlCommand cmd = new MySqlCommand(
                            @"SELECT COALESCE(SUM(total_amount), 0)
                              FROM order_items WHERE order_id=@id", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", payment.OrderId);
                            total = Convert.ToDecimal(cmd.ExecuteScalar());
                        }

                        // Calculate amount already paid
                        decimal paid;
                        using (MySqlCommand cmd = new MySqlCommand(
                            @"SELECT COALESCE(SUM(amount), 0)
                              FROM payments WHERE order_id=@id", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", payment.OrderId);
                            paid = Convert.ToDecimal(cmd.ExecuteScalar());
                        }

                        if (total <= 0 || paid + payment.Amount > total)
                            throw new Exception("Payment exceeds the unpaid order balance.");

                        // Save payment
                        string sql = @"INSERT INTO payments
                                       (order_id, amount, payment_method, payment_date)
                                       VALUES (@id, @amount, @method, NOW())";

                        using (MySqlCommand cmd = new MySqlCommand(sql, con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", payment.OrderId);
                            cmd.Parameters.AddWithValue("@amount", payment.Amount);
                            cmd.Parameters.AddWithValue("@method", payment.PaymentMethod);

                            cmd.ExecuteNonQuery();
                        }


                        string newStatus = (paid + payment.Amount >= total)
                            ? "Paid"
                            : "Partially Paid";

                        using (MySqlCommand cmd = new MySqlCommand(
                            "UPDATE orders SET status=@status WHERE order_id=@id",
                            con, tx))
                        {
                            cmd.Parameters.AddWithValue("@status", newStatus);
                            cmd.Parameters.AddWithValue("@id", payment.OrderId);
                            cmd.ExecuteNonQuery();
                        }


                        tx.Commit();

                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
