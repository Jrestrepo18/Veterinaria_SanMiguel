using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace veterinaria_san_miguel.Modals;

public class Veterinarian
{
    [Key]
    public int VeterinarianId { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; } // Agregado 'required'

    [Required]
    [MaxLength(50)]
    public required string Specialty { get; set; } // Agregado 'required'

    public ICollection<Appointment> PerformedAppointments { get; set; } = new List<Appointment>();
}