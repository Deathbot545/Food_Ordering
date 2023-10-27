using Core.DTO;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Orderser
{
    public interface IOrderService
    {
        Task UpdateOrderStatusAsync(Order updatedOrder);
        IEnumerable<OrderDTO> GetOrdersByOutletId(int outletId);
        // Add other order-related methods as needed
    }
}
