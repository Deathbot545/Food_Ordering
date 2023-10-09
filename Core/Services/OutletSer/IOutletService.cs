using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.OutletSer
{
    public interface IOutletService
    {
        Task<Outlet> RegisterOutletAsync(Outlet outlet, string currentUserId);
        QRCode GenerateQRCodeForOutlet(int outletId );
        Task<Outlet> GetOutletWithQRCodeAsync(int outletId);
        Task<List<Outlet>> GetOutletsByOwner(Guid ownerId);
        // Add other methods related to Outlet management
    }

}
