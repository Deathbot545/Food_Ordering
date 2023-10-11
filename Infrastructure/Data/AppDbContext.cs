using Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser> // Accessibility level should match or be less restrictive than AppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // Your DbSets and configurations go here
        public DbSet<Outlet> Outlets { get; set; }  // Make it public
        public DbSet<QRCode> QRCodes { get; set; }  // Make it public
        public DbSet<Table> Tables { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

          
        }
    }
}
