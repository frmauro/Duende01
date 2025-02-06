using Microsoft.EntityFrameworkCore;

namespace OrderApi.Model.Context;

public class MySqlContext : DbContext
{
    public MySqlContext() { }
    public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = "Server=localhost;DataBase=geek_shopping_order_api;Uid=root;Pwd=123";
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(connection, new MySqlServerVersion(new Version(5, 7)));
        }
    }
    public DbSet<OrderHeader> Headers { get; set; }
    public DbSet<OrderDetail> Details { get; set; }
}
