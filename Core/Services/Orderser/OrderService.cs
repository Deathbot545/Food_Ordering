using Infrastructure.Data;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Orderser
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context; // Assuming you have a DbContext named ApplicationDbContext

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateOrderStatusAsync(Order updatedOrder)
        {
            // Retrieve the existing order from the database
            var existingOrder = await _context.Orders.FindAsync(updatedOrder.Id);
            if (existingOrder == null)
            {
                throw new Exception("Order not found.");
            }

            // Update the status
            existingOrder.Status = updatedOrder.Status;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }
    }

}
