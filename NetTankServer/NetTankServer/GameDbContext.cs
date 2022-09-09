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
    public DbSet<PlayerTankEntity> PlayerTanks{ get; set; } = null!;
    public DbSet<TankEntity> Tanks { get; set; } = null!;

    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PlayerTankEntity>()
            .HasKey(e => new { e.PlayerId, e.TankId });
    }
}
