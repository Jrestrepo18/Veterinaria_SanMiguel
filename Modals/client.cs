namespace veterinaria_san_miguel.Modals;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Client
{
    [Key]
    public int ClientId { get; set; }

    [Required]
    [MaxLength(50)] // Ajuste el tamaño según necesite
    public required string FirstName { get; set; } // Nombre

    [Required]
    [MaxLength(50)] // Ajuste el tamaño según necesite
    public required string LastName { get; set; } // Apellido

    [Required]
    [MaxLength(20)] 
    public required string Identification { get; set; } // Identificación

    public required string Phone { get; set; } // Teléfono

    [Required]
    [MaxLength(100)]
    public required string Email { get; set; } // Correo Electrónico

    // Fechas
    public DateTime RegistrationDate { get; set; } = DateTime.Now; // Fecha de Registro (Completa)
    public DateTime RegisterDate { get; set; } = DateTime.Now; // Usado para propósitos históricos si es diferente de RegistrationDate.

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}