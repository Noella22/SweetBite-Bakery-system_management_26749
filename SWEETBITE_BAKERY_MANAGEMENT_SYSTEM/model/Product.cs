using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }
    }
}
