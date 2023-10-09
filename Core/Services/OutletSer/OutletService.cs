using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace Core.Services.OutletSer
{
    public class OutletService : IOutletService
    {
        private readonly AppDbContext _context;

        public OutletService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Outlet>> GetOutletsByOwner(Guid ownerId)
        {
            return await _context.Outlets.Where(x => x.OwnerId == ownerId && !x.IsDeleted).ToListAsync();
        }

        public async Task<Outlet> RegisterOutletAsync(Outlet outlet, string currentUserId)
        {
            if (outlet == null)
            {
                throw new ArgumentNullException(nameof(outlet));
            }

            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new ArgumentNullException(nameof(currentUserId));
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Set the OwnerId to the current user's ID
                    outlet.OwnerId = Guid.Parse(currentUserId);

                    // Example, assuming Outlet has a DateTime field named CreatedAt and UpdatedAt
                    // Ensure DateTime is in UTC
                    outlet.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                    outlet.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                    // Save Outlet to database to generate ID
                    await _context.Outlets.AddAsync(outlet);
                    SetDateTimePropertiesToUtc(outlet);
                    await _context.SaveChangesAsync();

                    // Now the outlet should have an ID
                    // Generate QR code
                    QRCode qrCode = GenerateQRCodeForOutlet(outlet.Id);

                    // Attach QR code entity to the outlet
                    outlet.QRCode = qrCode;

                    // Save QRCode to database
                    await _context.QRCodes.AddAsync(qrCode);

                    // Update Outlet to associate the new QRCode
                    _context.Outlets.Update(outlet);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return outlet;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception or handle it
                    throw;
                }
            }
        }



        public QRCode GenerateQRCodeForOutlet(int outletId)
        {
            var qrCodeWriter = new BarcodeWriterSvg
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300
                }
            };

            var urlToEncode = $"https://localhost:7115/api/OutletApi/GetOutletsByOwner/{outletId}";
            var svgContent = qrCodeWriter.Write(urlToEncode);

            var qrCode = new QRCode
            {
                Data = Encoding.UTF8.GetBytes(svgContent.Content),
                MimeType = "image/svg+xml"
            };

            return qrCode;
        }
        public static void SetDateTimePropertiesToUtc(object obj)
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(DateTime))
                {
                    var dt = (DateTime)property.GetValue(obj);
                    if (dt.Kind == DateTimeKind.Unspecified)
                    {
                        property.SetValue(obj, DateTime.SpecifyKind(dt, DateTimeKind.Utc));
                    }
                }
            }
        }
        public async Task<Outlet> GetOutletWithQRCodeAsync(int outletId)
        {
            if (outletId <= 0)
            {
                throw new ArgumentException("Invalid outlet ID", nameof(outletId));
            }

            try
            {
                var outlet = await _context.Outlets
                                           .Include(o => o.QRCode) // Include the QRCode
                                           .FirstOrDefaultAsync(o => o.Id == outletId && !o.IsDeleted);

                if (outlet == null)
                {
                    // Log or handle not found situation
                    return null;
                }

                return outlet;
            }
            catch (Exception ex)
            {
                // Log the exception
                // E.g., Console.WriteLine($"General Exception: {ex.Message}");
                throw;
            }
        }

    }

}
