using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTankServer;

public class GameDbContextDesignFactory : IDesignTimeDbContextFactory<GameDbContext>
{
    public GameDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder();
        var cnf = builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .Add(new JsonConfigurationSource
            {
                Path = "appsettings.json",
                ReloadOnChange = true,
            }).Build();
        var opb = new DbContextOptionsBuilder<GameDbContext>();
        opb.UseMySQL(cnf.GetConnectionString("Main"));
        return new GameDbContext(opb.Options);
    }
}
