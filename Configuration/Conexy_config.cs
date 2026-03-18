using ConexyTask.Entity;
using ConexyTask.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConexyTask.Configuration;

public class Conexy_config : IEntityTypeConfiguration<ConexyEntity>
{
    public void Configure(EntityTypeBuilder<ConexyEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(t => t.Name)
            .HasMaxLength(Conexy.MAX_LENGTH)
            .IsRequired();
        
        builder.Property(t => t.Description)
            .HasMaxLength(Conexy.MAX_LENGTH)
            .IsRequired();
    }
    
}