using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Added for DbContext operations
using veterinaria_san_miguel.Data;
using veterinaria_san_miguel.Modals;
using static System.Console;

public class VeterinarianService
{
    // 1. Register Veterinarian
    public void RegisterVeterinarian(Veterinarian veterinarian)
    {
        using (var context = new MysqlDbContext())
        {
            context.Veterinarians.Add(veterinarian);
            context.SaveChanges();
            WriteLine($"Veterinarian {veterinarian.Name} registered.");
        }
    }

    // NEW: 1.5 Get Veterinarian by ID (Crucial for Edit/Delete Handlers)
    public Veterinarian? GetVeterinarianById(int veterinarianId)
    {
        using (var context = new MysqlDbContext())
        {
            // Find is the fastest way to look up by primary key
            return context.Veterinarians.Find(veterinarianId);
        }
    }

    // 2. List Veterinarians
    public List<Veterinarian> ListVeterinarians()
    {
        using (var context = new MysqlDbContext())
        {
            return context.Veterinarians.ToList();
        }
    }

    // 3. Edit Veterinarian - IMPLEMENTED
    public void EditVeterinarian(int veterinarianId, string newName, string newSpecialty)
    {
        using (var context = new MysqlDbContext())
        {
            var veterinarian = context.Veterinarians.Find(veterinarianId);
            if (veterinarian != null)
            {
                veterinarian.Name = newName;
                veterinarian.Specialty = newSpecialty;
                context.SaveChanges();
                WriteLine($"Veterinarian ID {veterinarianId} updated.");
            }
            else
            {
                WriteLine($"Veterinarian ID {veterinarianId} not found.");
            }
        }
    }

    // 4. Delete Veterinarian - IMPLEMENTED
    public void DeleteVeterinarian(int veterinarianId)
    {
        using (var context = new MysqlDbContext())
        {
            var veterinarian = context.Veterinarians.Find(veterinarianId);
            if (veterinarian != null)
            {
                context.Veterinarians.Remove(veterinarian);
                context.SaveChanges();
                WriteLine($"Veterinarian ID {veterinarianId} deleted.");
            }
            else
            {
                WriteLine($"Veterinarian ID {veterinarianId} not found.");
            }
        }
    }
}