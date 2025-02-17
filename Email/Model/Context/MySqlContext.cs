using Microsoft.EntityFrameworkCore;

namespace Email.Model.Context;

public class MySqlContext : DbContext
{
    public MySqlContext() { }
    public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = "Server=localhost;DataBase=geek_shopping_email;Uid=root;Pwd=123";
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(connection, new MySqlServerVersion(new Version(5, 7)));
        }
    }
    public DbSet<EmailLog> Emails { get; set; }
}
