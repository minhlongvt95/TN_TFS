using Microsoft.EntityFrameworkCore;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Databases
{
    public partial class TenantContext : DbContext
    {
        public TenantContext()
        {
        }

        public TenantContext(DbContextOptions<TenantContext> options) : base(options)
        {
        }

        public virtual DbSet<Tenants> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenants>(entity =>
            {
                entity.HasKey(e => e.TenantId);

                entity.Property(e => e.TenantId)
                    .HasColumnName("TenantID")
                    .ValueGeneratedNever();

                entity.Property(e => e.TenantHost)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TenantName)
                    .IsRequired()
                    .HasMaxLength(500);
            });
        }
    }
}
