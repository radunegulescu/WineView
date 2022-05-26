using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WineView.Models;

namespace WineView.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Winery> Wineries { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Grape> Grapes { get; set; }
        public DbSet<Style> Styles { get; set; }
        public DbSet<Wine> Wines { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Body> Bodies { get; set; }
        public DbSet<Review> Reviews { get; set; }

    }
}
