using System;
using System.Data;
using MySqlConnector;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller
{
    public static class PaymentController
    {
        public static DataTable GetPayments()
        {
            PaymentRepository repo = new PaymentRepository();
            return repo.GetAll();
        }

        public static void Save(int orderId, string method, decimal amount)
        {
            if (orderId <= 0 || amount <= 0 ||
                string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException("Enter valid payment details.");
            }

            Payment payment = new Payment();
            payment.OrderId = orderId;
            payment.PaymentMethod = method;
            payment.Amount = amount;

            PaymentRepository repo = new PaymentRepository();
            repo.Insert(payment);
        }
    }
}
