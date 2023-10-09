using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class QRCode
    {
        [Key]  // Key annotation if this is going to be a separate table
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public string MimeType { get; set; }
        public Outlet? Outlet { get; set; } // Add this line

        // Additional properties
    }
}
