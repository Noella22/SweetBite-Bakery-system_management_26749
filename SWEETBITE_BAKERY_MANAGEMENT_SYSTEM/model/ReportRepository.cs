using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySqlConnector;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public class ReportRepository
    {
        public DataTable SalesByProduct()
        {
            string sql = @"SELECT product_name AS Product,
                           SUM(quantity) AS UnitsSold,
                           SUM(total_amount) AS RevenueRWF
                           FROM sales
                           GROUP BY product_name
                           ORDER BY RevenueRWF DESC";

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
    }
}

