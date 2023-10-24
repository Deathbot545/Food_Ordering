using Core.DTO;
using Core.Services.MenuS;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.CartSter
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly IMenuService _menuService;

        public CartService(AppDbContext context,IMenuService menuService)
        {
            _context = context;
            _menuService = menuService;
        }

        public async Task ProcessCartRequestAsync(CartRequest request)
        {
            foreach (var item in request.MenuItems)
            {
                var menuItemDto = await _menuService.GetMenuItemByIdAsync(item.Id);
                if (menuItemDto == null)
                {
                    throw new Exception($"MenuItem with Id {item.Id} not found.");
                }

                MenuItem menuItem = new MenuItem
                {
                    Id = menuItemDto.id,
                    Name = menuItemDto.Name,
                    Description = menuItemDto.Description,
                    Price = menuItemDto.Price,
                    MenuCategoryId = menuItemDto.MenuCategoryId,
                    Image = Convert.FromBase64String(menuItemDto.Image)
                };
                await AddToCartAsync(menuItem, item.Qty, request.UserId, request.TableId); // Pass the TableId here
            }
        }

        public async Task AddToCartAsync(MenuItem menuItem, int quantity, string userId = null, int tableId = 0) // Add tableId parameter here
        {
            if (string.IsNullOrEmpty(userId))
            {
                await AddToCartAsGuestAsync(menuItem, quantity, tableId);
            }
            else
            {
                await AddToCartAsAuthenticatedUserAsync(menuItem, quantity, userId, tableId);
            }
        }

        private async Task AddToCartAsGuestAsync(MenuItem menuItem, int quantity, int tableId)
        {
            var currentOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Status == OrderStatus.Pending && o.Customer == null)
                .FirstOrDefaultAsync();

            if (currentOrder == null)
            {
                currentOrder = new Order
                {
                    OrderTime = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TableId = tableId // Save the TableId here
                };

                _context.Orders.Add(currentOrder);
            }

            var orderDetail = new OrderDetail
            {
                Order = currentOrder,
                MenuItemId = menuItem.Id, // Use the foreign key directly
                Quantity = quantity
            };

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
        }


        private async Task AddToCartAsAuthenticatedUserAsync(MenuItem menuItem, int quantity, string userId, int tableId)
        {
            var currentOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Customer.Id == userId && o.Status == OrderStatus.Pending)
                .FirstOrDefaultAsync();

            if (currentOrder == null)
            {
                currentOrder = new Order
                {
                    OrderTime = DateTime.UtcNow,
                    Customer = await _context.Users.FindAsync(userId),
                    Status = OrderStatus.Pending,
                    TableId = tableId // Save the TableId here
                };

                _context.Orders.Add(currentOrder);
            }

            var orderDetail = new OrderDetail
            {
                Order = currentOrder,
                MenuItemId = menuItem.Id, // Use the foreign key directly
                Quantity = quantity
            };

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
        }

    }

}
