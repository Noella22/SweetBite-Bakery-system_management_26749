
using System;
using System.Security.Cryptography;
using MySqlConnector;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller
{
    public static class AuthController
    {
        
        public static bool HasAccounts()
        {
            if (!SchemaController.HasTable("users"))
                return false;

            using (MySqlConnection con =
                new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM users", con))
            {
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        
        public static void CreateFirstAccount(
            string username, string password)
        {
            if (!SchemaController.HasTable("users"))
                throw new Exception(
                    "Please import sweetbite_db.sql first.");

            if (HasAccounts())
                throw new Exception(
                    "The first account already exists.");

            SaveAccount(username, password, "Admin");
        }

        
        public static void CreateAccount(
            string username, string password)
        {
            if (!SchemaController.HasTable("users"))
                throw new Exception(
                    "Please import sweetbite_db.sql first.");

            
            if (!HasAccounts())
            {
                CreateFirstAccount(username, password);
                return;
            }

            
            SaveAccount(username, password, "Staff");
        }

        
        private static void SaveAccount(
            string username, string password, string role)
        {
            username = username.Trim();

            if (username == "")
                throw new Exception("Enter a username.");

            if (password == null || password.Length < 8)
                throw new Exception(
                    "Password must have at least 8 characters.");

            byte[] salt = new byte[16];

            using (RandomNumberGenerator random =
                RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            byte[] hash;

            using (Rfc2898DeriveBytes derive =
                new Rfc2898DeriveBytes(password, salt, 150000))
            {
                hash = derive.GetBytes(32);
            }

            string stored =
                Convert.ToBase64String(salt) + ":" +
                Convert.ToBase64String(hash);

            using (MySqlConnection con =
                new MySqlConnection(Database.ConnectionString))
            {
                con.Open();

                
                using (MySqlCommand check = new MySqlCommand(
                    "SELECT COUNT(*) FROM users WHERE username=@name",
                    con))
                {
                    check.Parameters.AddWithValue(
                        "@name", username);

                    if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                    {
                        throw new Exception(
                            "This username already exists.");
                    }
                }

                
                using (MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO users " +
                    "(username, password_hash, role) " +
                    "VALUES (@name, @hash, @role)", con))
                {
                    cmd.Parameters.AddWithValue(
                        "@name", username);

                    cmd.Parameters.AddWithValue(
                        "@hash", stored);

                    cmd.Parameters.AddWithValue(
                        "@role", role);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        
        public static bool Login(
            string username, string password)
        {
            if (!SchemaController.HasTable("users"))
                return false;

            object value;

            using (MySqlConnection con =
                new MySqlConnection(Database.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT password_hash FROM users " +
                "WHERE username=@name", con))
            {
                cmd.Parameters.AddWithValue(
                    "@name", username.Trim());

                con.Open();
                value = cmd.ExecuteScalar();
            }

            if (value == null || value == DBNull.Value)
                return false;

            string[] parts = Convert.ToString(value).Split(':');

            if (parts.Length != 2)
                return false;

            try
            {
                byte[] salt =
                    Convert.FromBase64String(parts[0]);

                byte[] expected =
                    Convert.FromBase64String(parts[1]);

                byte[] actual;

                using (Rfc2898DeriveBytes derive =
                    new Rfc2898DeriveBytes(
                        password, salt, 150000))
                {
                    actual = derive.GetBytes(expected.Length);
                }

                int different = 0;

                for (int i = 0; i < expected.Length; i++)
                {
                    different |= expected[i] ^ actual[i];
                }

                return different == 0;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
