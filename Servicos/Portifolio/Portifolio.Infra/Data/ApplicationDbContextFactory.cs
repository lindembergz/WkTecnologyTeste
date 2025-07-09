using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Portifolio.Infraestrutura.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<PortifolioDbContext>
{
    public PortifolioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PortifolioDbContext>();
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3306;Database=vehiclesalesdb;User=root;Password=RootPassword123;",
            ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=vehiclesalesdb;User=root;Password=RootPassword123;"));
        return new PortifolioDbContext(optionsBuilder.Options);
    }
}
