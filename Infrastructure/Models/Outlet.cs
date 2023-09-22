using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Outlet
    {
        [Key] public int Id { get; set; }

        [Required(ErrorMessage = "Outlet Name (Internal) is required.")]
        public string InternalOutletName { get; set; }

        [Required(ErrorMessage = "Customer-Facing Name is required.")]
        public string CustomerFacingName { get; set; }

        [Required(ErrorMessage = "Business Type is required.")]
        public string BusinessType { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip is required.")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Please provide a valid 5-digit zip code.")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Street Address is required.")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "Postal/ZIP Code is required.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Date Opened is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOpened { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public bool HealthAndSafetyCompliance { get; set; }

        [Required(ErrorMessage = "Employee Count is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee Count must be a positive number.")]
        public int EmployeeCount { get; set; }

        [Required(ErrorMessage = "Operating Hours Start is required.")]
        [DataType(DataType.Time)]
        public TimeSpan OperatingHoursStart { get; set; }

        [Required(ErrorMessage = "Operating Hours End is required.")]
        [DataType(DataType.Time)]
        public TimeSpan OperatingHoursEnd { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL for the Contact.")]
        public string Contact { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL for the Facebook Page.")]
        public string Facebook { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL for the Twitter Profile.")]
        public string Twitter { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL for the Instagram Profile.")]
        public string Instagram { get; set; }

        // You may add more properties as needed, such as LogoFile for file uploads.

        [Required(ErrorMessage = "You must agree to the terms and conditions.")]
        public bool AgreeToTerms { get; set; }

        // New property for QR code data
        public byte[] QRCodeData { get; set; }
    }
}
