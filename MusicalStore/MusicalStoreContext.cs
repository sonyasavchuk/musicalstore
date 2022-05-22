using Microsoft.EntityFrameworkCore;
using MusicalStore.Models.Entities;

namespace MusicalStore;

public class MusicalStoreContext : DbContext
{
    public MusicalStoreContext()
    {}

    public MusicalStoreContext(DbContextOptions<MusicalStoreContext> options)
        : base(options)
    {}

    public DbSet<Country> Countries { get; set; }
        
    public DbSet<Group> Groups { get; set; }
        
    public DbSet<Instrument> Instruments { get; set; }

    public DbSet<Manufacturer> Manufacturers { get; set; }
        
    public DbSet<Material> Materials { get; set; }
        
    public DbSet<InstrumentType> Types { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.CountryName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(e => e.GroupName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Instrument>(entity =>
        {
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.InstrumentType)
                .WithMany(p => p.Instruments)
                .HasForeignKey(d => d.InstrumentTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Instruments_InstrumentType");

            entity.HasOne(d => d.ManufactoringCountry)
                .WithMany(p => p.Instruments)
                .HasForeignKey(d => d.ManufactoringCountryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Instruments_Country");

            entity.HasOne(d => d.Manufacturer)
                .WithMany(p => p.Instruments)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Instruments_Manufacturer");

            entity.HasMany(x => x.Materials)
                .WithMany(x => x.Instruments)
                .UsingEntity<InstrumentMaterial>(
                configureRight => configureRight
                    .HasOne(d => d.Material)
                    .WithMany()
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Instrument/Materials_Materials"),
                configureLeft => configureLeft
                    .HasOne(d => d.Instrument)
                    .WithMany()
                    .HasForeignKey(d => d.InstrumentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Instrument/Materials_Instruments")
                );
        });

        modelBuilder.Entity<Manufacturer>(entity => {
            entity.Property(e => e.ManufacturerName)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Country)
                .WithMany(p => p.Manufacturers)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Manufacturer_Country");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.Property(e => e.MaterialName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<InstrumentType>(entity =>
        {
            entity.Property(e => e.TypeName)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Group)
                .WithMany(p => p.Types)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_InstrumentType_InstrumentGroup");
        });
    }
}