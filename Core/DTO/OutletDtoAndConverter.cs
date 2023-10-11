using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
   /* public class OutletDtoAndConverter
    {
        public class OutletDto
        {
            public int Id { get; set; }
            public string CustomerFacingName { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Contact { get; set; }
            public TimeSpan OperatingHoursStart { get; set; }
            public TimeSpan OperatingHoursEnd { get; set; }
            public string QRCodeData { get; set; } // Now it's a string
        }

        public static OutletDto ConvertToDto(Outlet outlet)
        {
            return new OutletDto
            {
                Id = outlet.Id,
                CustomerFacingName = outlet.CustomerFacingName,
                City = outlet.City,
                State = outlet.State,
                Contact = outlet.Contact,
                OperatingHoursStart = outlet.OperatingHoursStart,
                OperatingHoursEnd = outlet.OperatingHoursEnd,
                QRCodeData = Encoding.UTF8.GetString(outlet.QRCode?.Data ?? new byte[0])
                // Convert byte array to string. Adapt this line to your actual scenario.
            };
        }
    }*/
}
