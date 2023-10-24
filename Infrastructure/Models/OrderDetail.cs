using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        // Foreign key properties
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }
    }


}
