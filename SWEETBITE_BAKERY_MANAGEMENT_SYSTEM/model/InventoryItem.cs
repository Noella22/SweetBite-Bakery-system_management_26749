using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.model
{
    public class InventoryItem
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public int QuantityInStock { get; set; }
    }
}

