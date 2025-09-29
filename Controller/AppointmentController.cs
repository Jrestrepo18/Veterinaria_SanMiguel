using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using veterinaria_san_miguel.Data;
using veterinaria_san_miguel.Modals;
using static System.Console;

public class AppointmentService
{
    // 1. Register Appointment 
    public void RegisterAppointment(DateTime date, string diagnosis, int petId, int veterinarianId)
    {
        var appointment = new Appointment
        {
            Date = date,
            Diagnosis = diagnosis,
            PetId = petId,
            VeterinarianId = veterinarianId,
            // NOTE: Setting navigation properties to null is fine since FKs are set.
            Pet = null,
            Veterinarian = null
        };

        using (var context = new MysqlDbContext())
        {
            context.Appointments.Add(appointment);
            context.SaveChanges();
            WriteLine($"Appointment registered on {date.ToShortDateString()}.");
        }
    }
    
    // NEW: 1.5 Get Appointment by ID (Crucial for Edit/Delete Handlers)
    public Appointment? GetAppointmentById(int appointmentId)
    {
        using (var context = new MysqlDbContext())
        {
            return context.Appointments
                // Eagerly load related entities so the UI can display names
                .Include(a => a.Pet)
                .Include(a => a.Veterinarian)
                .FirstOrDefault(a => a.AppointmentId == appointmentId);
        }
    }

    // 2. List All Appointments (Eager loading Pet and Veterinarian)
    public List<Appointment> ListAppointments()
    {
        using (var context = new MysqlDbContext())
        {
            return context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Veterinarian)
                .ToList();
        }
    }
    
    // 3. Edit Appointment (Diagnosis only)
    public void EditAppointment(int appointmentId, string newDiagnosis) 
    { 
        using (var context = new MysqlDbContext())
        {
            var appointment = context.Appointments.Find(appointmentId);
            if (appointment != null)
            {
                appointment.Diagnosis = newDiagnosis;
                context.SaveChanges();
                WriteLine($"Appointment ID {appointmentId} diagnosis updated.");
            }
            else
            {
                WriteLine($"Appointment ID {appointmentId} not found.");
            }
        }
    }

    // 4. Delete Appointment
    public void DeleteAppointment(int appointmentId) 
    { 
        using (var context = new MysqlDbContext())
        {
            var appointment = context.Appointments.Find(appointmentId);
            if (appointment != null)
            {
                context.Appointments.Remove(appointment);
                context.SaveChanges();
                WriteLine($"Appointment ID {appointmentId} deleted.");
            }
            else
            {
                WriteLine($"Appointment ID {appointmentId} not found.");
            }
        }
    }
    
    // 5. Medical History
    public List<Appointment> GetMedicalHistory(int petId)
    {
        using (var context = new MysqlDbContext())
        {
            return context.Appointments
                .Include(a => a.Veterinarian) 
                .Where(a => a.PetId == petId)
                .OrderByDescending(a => a.Date)
                .ToList();
        }
    }

    // Advanced Queries 

    // 6. Get all pets of a client
    public List<Pet> GetPetsByClient(int clientId)
    {
        using (var context = new MysqlDbContext())
        {
            return context.Pets
                .Where(m => m.ClientId == clientId)
                .ToList();
        }
    }

    // 7. Get the veterinarian with the most appointments. (Return type set to nullable)
    public Veterinarian? GetBusiestVeterinarian()
    {
        using (var context = new MysqlDbContext())
        {
            // The logic correctly uses FirstOrDefault, so the return type should be nullable.
            return context.Appointments
                .GroupBy(a => a.VeterinarianId)
                .Select(g => new { VeterinarianId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(1)
                .Join(context.Veterinarians, 
                      at => at.VeterinarianId,
                      vet => vet.VeterinarianId,
                      (at, vet) => vet)
                .FirstOrDefault();
        }
    }

    // 8. Get the most frequent pet species in appointments (Return type set to nullable)
    public string? GetMostAttendedSpecies()
    {
        using (var context = new MysqlDbContext())
        {
            return context.Appointments
                .Join(context.Pets, a => a.PetId, m => m.PetId, (a, m) => m.Species)
                .GroupBy(species => species)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
        }
    }
    
    // 9. Get the client with the most registered pets (Return type set to nullable)
    public Client? GetClientWithMostPets()
    {
        using (var context = new MysqlDbContext())
        {
            return context.Pets
                .GroupBy(m => m.ClientId)
                .Select(g => new { ClientId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(1)
                .Join(context.Clients, 
                      m => m.ClientId, 
                      c => c.ClientId, 
                      (m, c) => c)
                .FirstOrDefault();
        }
    }
}