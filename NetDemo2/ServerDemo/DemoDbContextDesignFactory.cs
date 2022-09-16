using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDemo;

public class DemoDbContextDesignFactory : IDesignTimeDbContextFactory<DemoDbContext>
{
    public DemoDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder();
        var cnf = builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .Add(new JsonConfigurationSource
            {
                Path = "appsettings.json",
                ReloadOnChange = true,
            }).Build();
        var opb = new DbContextOptionsBuilder<DemoDbContext>();
        opb.UseMySQL(cnf.GetConnectionString("Main"));
        return new DemoDbContext(opb.Options);
    }
}
