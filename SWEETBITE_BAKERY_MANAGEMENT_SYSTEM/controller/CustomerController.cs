using System;
using System.Data;
using MySqlConnector;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller
{
    public static class CustomerController
    {
        // Display and search customers
        public static DataTable GetCustomers(string search)
        {
            CustomerRepository repo = new CustomerRepository();
            return repo.Search(search ?? "");
        }

        // Save or update customer
        public static void Save(int id, string name, string phone)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
               
            {
                throw new ArgumentException("Enter customer name and phone.");
            }

            if (id < 0)
                throw new ArgumentException("Invalid customer ID.");

            Customer customer = new Customer();
            customer.CustomerId = id;
            customer.CustomerName = name.Trim();
            customer.Phone = phone.Trim();
            customer.Email = "";

            CustomerRepository repo = new CustomerRepository();

            if (id == 0)
                repo.Insert(customer);
            else
                repo.Update(customer);
        }

        // Delete customer
        public static void Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Select a customer first.");

            CustomerRepository repo = new CustomerRepository();
            repo.Delete(id);
        }
    }
}
