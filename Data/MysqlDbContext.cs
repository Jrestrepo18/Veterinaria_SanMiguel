using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using veterinaria_san_miguel.Modals;
using System;
 
namespace veterinaria_san_miguel.Data;

public class MysqlDbContext : DbContext
{
    // DBSETS Updated to use English names
    public DbSet<Client> Clients { get; set; } // Renamed from Clientes
    public DbSet<Pet> Pets { get; set; } // Renamed from Mascotas
    public DbSet<Veterinarian> Veterinarians { get; set; } // Renamed from Veterinarios
    public DbSet<Appointment> Appointments { get; set; } // Renamed from Atenciones

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Connection to MySQL
        const string connectionString =
            "Server=168.119.183.3;Database=jeronimo_cardona;User=root;Password=g0tIFJEQsKHm5$34Pxu1;Port=3307";

        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Client (1) to Pet (Many)
        modelBuilder.Entity<Pet>()
            .HasOne(m => m.Client)
            .WithMany(c => c.Pets)
            .HasForeignKey(m => m.ClientId);

        // Pet (1) to Appointment (Many)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Pet)
            .WithMany() 
            .HasForeignKey(a => a.PetId);

        // Veterinarian (1) to Appointment (Many)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Veterinarian)
            .WithMany(v => v.PerformedAppointments)
            .HasForeignKey(a => a.VeterinarianId);

        base.OnModelCreating(modelBuilder);
    }
}