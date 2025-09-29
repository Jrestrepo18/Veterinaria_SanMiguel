using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using veterinaria_san_miguel.Data;
using veterinaria_san_miguel.Modals;
using static System.Console;

public class PetService
{
    // 1. Register Pet 
    public void RegisterPet(Pet pet, int clientId)
    {
        using (var context = new MysqlDbContext())
        {
            pet.ClientId = clientId; 
            context.Pets.Add(pet);
            context.SaveChanges();
            WriteLine($"Pet {pet.Name} registered.");
        }
    }

    // NEW: 1.5 Get Pet by ID (Crucial for Edit/Delete Handlers)
    public Pet? GetPetById(int petId)
    {
        using (var context = new MysqlDbContext())
        {
            // Eagerly load the Client details (FirstName, LastName) for display in Program.cs
            return context.Pets
                .Include(m => m.Client)
                .FirstOrDefault(m => m.PetId == petId);
        }
    }

    // 2. List Pets (Overload 1: List All)
    public List<Pet> ListPets()
    {
        using (var context = new MysqlDbContext())
        {
            // Include Client for displaying owner's name
            return context.Pets.Include(m => m.Client).ToList();
        }
    }
    
    // Overload 2: List Pets by Species
    public List<Pet> ListPets(string species)
    {
        using (var context = new MysqlDbContext())
        {
            return context.Pets
                .Include(m => m.Client) // Also include client here for consistency
                .Where(m => m.Species == species)
                .ToList();
        }
    }

    // 3. Edit Pet - IMPLEMENTED
    public void EditPet(int petId, string newName, string newBreed)
    {
        using (var context = new MysqlDbContext())
        {
            var pet = context.Pets.Find(petId);
            if (pet != null)
            {
                pet.Name = newName;
                pet.Breed = newBreed;
                context.SaveChanges();
                WriteLine($"Pet ID {petId} updated.");
            }
            else
            {
                WriteLine($"Pet ID {petId} not found.");
            }
        }
    }

    // 4. Delete Pet - IMPLEMENTED
    public void DeletePet(int petId)
    {
        using (var context = new MysqlDbContext())
        {
            var pet = context.Pets.Find(petId);
            if (pet != null)
            {
                context.Pets.Remove(pet);
                context.SaveChanges();
                WriteLine($"Pet ID {petId} deleted.");
            }
            else
            {
                WriteLine($"Pet ID {petId} not found.");
            }
        }
    }
}
