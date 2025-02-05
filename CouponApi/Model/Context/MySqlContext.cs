using Microsoft.EntityFrameworkCore;

namespace CouponApi.Model.Context;

public class MySqlContext : DbContext
{
    public MySqlContext() { }
    public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = "Server=localhost;DataBase=geek_shopping_coupon_api;Uid=root;Pwd=123";
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(connection, new MySqlServerVersion(new Version(5, 7)));
        }
    }
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            Id = 1,
            CouponCode = "ERUDIO_2022_10",
            DiscountAmount = 10
        });
        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            Id = 2,
            CouponCode = "ERUDIO_2022_15",
            DiscountAmount = 15
        });
    }
}
