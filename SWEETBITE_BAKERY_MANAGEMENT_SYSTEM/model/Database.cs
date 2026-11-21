using System.Configuration;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public static class Database
    {
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["SweetBiteDB"].ConnectionString; }
        }
    }
}
