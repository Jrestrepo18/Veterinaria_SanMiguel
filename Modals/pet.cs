using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace veterinaria_san_miguel.Modals;

public class Pet
{
    [Key]
    public int PetId { get; set; }

    [Required]
    [MaxLength(100)] 
    public required string Name { get; set; }
    public required string Species { get; set; }
    public required string Breed { get; set; }

    // Foreign Key to Client
    public int ClientId { get; set; }

    // Propiedad de Navegación: Corregido con 'required'
    [Required]
    public required Client Client { get; set; } 
}