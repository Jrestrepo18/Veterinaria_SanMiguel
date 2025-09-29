using System.ComponentModel.DataAnnotations;

namespace veterinaria_san_miguel.Modals;

public class Appointment
{
    [Key]
    public int AppointmentId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    // El diagnóstico es requerido y no puede ser nulo
    [Required]
    public required string Diagnosis { get; set; } 

    // Foreign Keys and Navigation Properties
    
    // ID de la Mascota
    public int PetId { get; set; }
    // Propiedad de navegación: Cambiada a Pet? (nullable) para evitar el warning CS8625 al guardar solo el PetId.
    public Pet? Pet { get; set; } 

    // ID del Veterinario
    public int VeterinarianId { get; set; }
    // Propiedad de navegación: Cambiada a Veterinarian? (nullable) para evitar el warning CS8625 al guardar solo el VeterinarianId.
    public Veterinarian? Veterinarian { get; set; } 
}