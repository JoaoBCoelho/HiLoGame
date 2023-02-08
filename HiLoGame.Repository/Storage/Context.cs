using HiLoGame.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace HiLoGame.Repository.Storage;
[ExcludeFromCodeCoverage]
public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public void Migrate()
    {
        Database.Migrate();
    }

    public DbSet<Game> Games { get; set; }
    public DbSet<GamePlayerInfo> GamePlayerInfos { get; set; }
    public DbSet<Player> Players { get; set; }
}