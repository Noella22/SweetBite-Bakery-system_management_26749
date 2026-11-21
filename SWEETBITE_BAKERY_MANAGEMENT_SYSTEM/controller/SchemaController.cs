using System;
using MySqlConnector;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller
{
    public static class SchemaController
    {
        public static bool HasTable(string name)
        {
            const string sql = @"SELECT COUNT(*) FROM information_schema.tables
                WHERE table_schema = DATABASE() AND table_name = @name";
            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@name", name);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public static bool HasColumn(string table, string column)
        {
            const string sql = @"SELECT COUNT(*) FROM information_schema.columns
                WHERE table_schema = DATABASE() AND table_name = @table AND column_name = @column";
            using (MySqlConnection con = new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@table", table);
                cmd.Parameters.AddWithValue("@column", column);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}
