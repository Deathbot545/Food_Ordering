using Core.DTO;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.CartSter
{
    public interface ICartService
    {
        Task AddToCartAsync(MenuItem menuItem, int quantity, string userId = null, int tableId = 0, int outletId = 0);
        Task ProcessCartRequestAsync(CartRequest request);
        // Add other cart related methods if needed...
    }
}
