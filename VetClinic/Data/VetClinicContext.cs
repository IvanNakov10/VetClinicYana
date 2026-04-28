using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using VetClinic.Data.Models;

namespace VetClinic.Data
{
    /// <summary>
    /// The main EF Core database context for the Veterinary Clinic application.
    /// Configures all three tables and their relationships, and seeds initial data.
    /// </summary>
    public class VetClinicContext : DbContext
    {
        // ── Tables ─────────────────────────────────────────────────────────────
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Animal> Animals { get; set; }

        // ── Connection string (LocalDB) ────────────────────────────────────────
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=VetClinicDB;Trusted_Connection=True;");
        }

        // ── Relationships & seeding ────────────────────────────────────────────
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Owner ──────────────────────────────────────────────────────────
            modelBuilder.Entity<Owner>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.FullName).IsRequired().HasMaxLength(150);
                entity.Property(o => o.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(o => o.Email).HasMaxLength(200);

                // One Owner → One Address (Owner holds the FK)
                entity.HasOne(o => o.Address)
                      .WithOne(a => a.Owner)
                      .HasForeignKey<Owner>(o => o.AddressId)
                      .OnDelete(DeleteBehavior.SetNull);

                // One Owner → Many Animals
                entity.HasMany(o => o.Animals)
                      .WithOne(a => a.Owner)
                      .HasForeignKey(a => a.OwnerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Address ────────────────────────────────────────────────────────
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Country).IsRequired().HasMaxLength(100);
                entity.Property(a => a.City).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Street).IsRequired().HasMaxLength(200);
                entity.Property(a => a.PostalCode).IsRequired().HasMaxLength(20);
            });

            // ── Animal ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Animal>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Species).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Breed).IsRequired().HasMaxLength(100);
                entity.Property(a => a.MedicalNotes).HasMaxLength(1000);
            });

            // ── Seed data ──────────────────────────────────────────────────────
            // One default Address
            modelBuilder.Entity<Address>().HasData(new Address
            {
                Id = 1,
                Country = "Bulgaria",
                City = "Sofia",
                Street = "Vitosha Blvd 1",
                PostalCode = "1000"
            });

            // One default Owner linked to the address above
            modelBuilder.Entity<Owner>().HasData(new Owner
            {
                Id = 1,
                FullName = "Ivan Petrov",
                PhoneNumber = "+359 88 123 4567",
                Email = "ivan.petrov@example.com",
                AddressId = 1
            });

            // One default Animal linked to the owner above
            modelBuilder.Entity<Animal>().HasData(new Animal
            {
                Id = 1,
                Name = "Rex",
                Species = "Dog",
                Breed = "German Shepherd",
                Age = 3,
                MedicalNotes = "Healthy. Annual vaccinations up to date.",
                OwnerId = 1
            });
        }
    }
}