using System.Data;
using MySqlConnector;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model;


namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller
{
    public static class ReportController
    {
        public static DataTable SalesByProduct()
        {
            ReportRepository repo = new ReportRepository();
            return repo.SalesByProduct();
        }
    }
}
