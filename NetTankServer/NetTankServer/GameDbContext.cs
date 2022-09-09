using Microsoft.EntityFrameworkCore;
using NetTankServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTankServer;

public class GameDbContext : DbContext
{
    public DbSet<PlayerEntity> Players { get; set; } = null!;

    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    //
}
