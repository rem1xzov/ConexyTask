using ConexyTask.Configuration;
using ConexyTask.Entity;
using Microsoft.EntityFrameworkCore;

namespace ConexyTask.DbContext;

public class DbConexy : Microsoft.EntityFrameworkCore.DbContext
{
    public DbConexy(DbContextOptions<DbConexy> options):base(options){}
    
    public DbSet<ConexyEntity> ConexyEntities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new Conexy_config());
    }
}