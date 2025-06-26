using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Portifolio.Infraestrutura.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3306;Database=vehiclesalesdb;User=root;Password=RootPassword123;",
            ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=vehiclesalesdb;User=root;Password=RootPassword123;"));
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
