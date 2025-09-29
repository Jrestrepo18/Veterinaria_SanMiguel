using System;
using System.Linq;
using System.Collections.Generic;
using veterinaria_san_miguel.Modals;
using static System.Console;
using Microsoft.EntityFrameworkCore;
using veterinaria_san_miguel.Data;
// Nota: Se asume que las clases de servicio están accesibles
// using veterinaria_san_miguel.Services; 

public class Program
{
    // Service Instances
    // FIX: Aplicar 'readonly' a los campos de servicio según la advertencia.
    private static readonly ClientService _clientService = new ClientService();
    private static readonly PetService _petService = new PetService();
    private static readonly VeterinarianService _veterinarianService = new VeterinarianService();
    private static readonly AppointmentService _appointmentService = new AppointmentService();

    public static void Main(String[] args)
    {
        // Asegurar que la base de datos exista y las migraciones se apliquen.
        try
        {
            // NOTA: MysqlDbContext debe existir y ser accesible
            using (var context = new MysqlDbContext())
            {
                // Aplica migraciones pendientes o crea la base de datos si no existe
                context.Database.Migrate(); 
                WriteLine("✅ Base de datos verificada y migraciones aplicadas.");
            }
        }
        catch (Exception ex)
        {
            WriteLine("ERROR CRÍTICO al conectar/migrar la base de datos:");
            WriteLine(ex.Message);
            WriteLine("Asegúrese de que el servidor MySQL esté corriendo y las credenciales sean correctas.");
            Pausar();
            return; 
        }

        bool exit = false;

        while (!exit)
        {
            ShowMainMenu();
            string? option = ReadLine(); // ReadLine puede ser nulo.
            Clear();

            switch (option)
            {
                case "1":
                    ManageClientsMenu();
                    break;
                case "2":
                    ManagePetsMenu();
                    break;
                case "3":
                    ManageVeterinariansMenu();
                    break;
                case "4":
                    ManageAppointmentsMenu();
                    break;
                case "5":
                    MedicalHistoryMenu();
                    break;
                case "6":
                    AdvancedQueriesMenu();
                    break;
                case "0":
                    WriteLine("Gracias por usar el Sistema Veterinaria San Miguel. ¡Hasta pronto!");
                    exit = true;
                    break;
                default:
                    WriteLine("Opción no válida. Por favor, selecciona un número del menú.");
                    Pausar();
                    break;
            }
        }
    }

    public static void Pausar()
    {
        WriteLine("\nPresione cualquier tecla para continuar...");
        ReadKey();
    }
    
    // --- Menús y Handlers (Funciones) ---

    public static void ShowMainMenu()
    {
        Clear();
        WriteLine("==============================================");
        WriteLine("      SISTEMA VETERINARIA SAN MIGUEL     ");
        WriteLine("==============================================");
        WriteLine("1. Gestión de Clientes");
        WriteLine("2. Gestión de Mascotas");
        WriteLine("3. Gestión de Veterinarios");
        WriteLine("4. Gestión de Citas Médicas");
        WriteLine("5. Historial Médico de Mascotas");
        WriteLine("6. Consultas Avanzadas (LINQ sobre EF)");
        WriteLine("----------------------------------------------");
        WriteLine("0. Salir");
        WriteLine("==============================================");
        Write("Seleccione una opción: ");
    }

    // --- GESTIÓN DE CLIENTES ---

    public static void ManageClientsMenu()
    {
        bool back = false;
        while (!back)
        {
            Clear();
            WriteLine("--- GESTIÓN DE CLIENTES ---");
            WriteLine("1. Registrar Cliente");
            WriteLine("2. Listar Clientes");
            WriteLine("3. Editar Cliente");
            WriteLine("4. Eliminar Cliente");
            WriteLine("0. Volver al Menú Principal");
            Write("Opción: ");

            string? option = ReadLine();
            WriteLine("---------------------------------");

            switch (option)
            {
                case "1":
                    RegisterClientHandler();
                    break;
                case "2":
                    ListClientsHandler();
                    break;
                case "3":
                    EditClientHandler();
                    break;
                case "4":
                    DeleteClientHandler();
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    WriteLine("Opción no válida.");
                    Pausar();
                    break;
            }
        }
    }
    
    private static void RegisterClientHandler()
    {
        Write("Nombre del Cliente: ");
        string firstName = ReadLine() ?? string.Empty; 
        Write("Apellido del Cliente: ");
        string lastName = ReadLine() ?? string.Empty;
        Write("Identificación (C.C/NIT): ");
        string identification = ReadLine() ?? string.Empty;
        Write("Teléfono del Cliente: ");
        string phone = ReadLine() ?? string.Empty; 
        Write("Email del Cliente: ");
        string email = ReadLine() ?? string.Empty;

        var newClient = new Client 
        { 
            FirstName = firstName, 
            LastName = lastName, 
            Identification = identification,
            Phone = phone, 
            Email = email 
        };
        _clientService.RegisterClient(newClient);
        Pausar();
    }

    private static void ListClientsHandler()
    {
        var clients = _clientService.ListClients();
        if (clients.Any())
        {
            WriteLine("--- LISTADO DE CLIENTES ---");
            WriteLine($"{"ID",-5} {"NOMBRE COMPLETO",-35} {"IDENTIFICACIÓN",-20} {"TELÉFONO",-15} {"EMAIL"}");
            WriteLine(new string('-', 85));
            foreach (var c in clients)
            {
                string fullName = $"{c.FirstName} {c.LastName}";
                WriteLine($"{c.ClientId,-5} {fullName,-35} {c.Identification,-20} {c.Phone,-15} {c.Email}");
            }
        }
        else
        {
            WriteLine("No hay clientes registrados.");
        }
        Pausar();
    }

    // *** CLIENTE: Incluye la visualización de datos actuales antes de la edición ***
    private static void EditClientHandler()
    {
        Write("ID del cliente a editar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            var clientToEdit = _clientService.GetClientById(id);
            
            if (clientToEdit == null)
            {
                WriteLine($"Cliente ID {id} no encontrado.");
                Pausar();
                return;
            }

            // MOSTRAR INFORMACIÓN ACTUAL
            WriteLine("\n--- DATOS ACTUALES DEL CLIENTE ---");
            WriteLine($"Nombre Actual: {clientToEdit.FirstName}");
            WriteLine($"Apellido Actual: {clientToEdit.LastName}");
            WriteLine($"Identificación Actual: {clientToEdit.Identification}");
            WriteLine($"Teléfono Actual: {clientToEdit.Phone}");
            WriteLine($"Email Actual: {clientToEdit.Email}");
            WriteLine($"Fecha Registro: {clientToEdit.RegistrationDate:dd/MM/yyyy}");
            WriteLine("----------------------------------");
            
            WriteLine("\nIngrese nuevos valores (deje vacío y presione Enter para mantener el valor actual):");

            Write($"Nuevo Nombre ({clientToEdit.FirstName}): ");
            string newFirstName = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newFirstName)) newFirstName = clientToEdit.FirstName;

            Write($"Nuevo Apellido ({clientToEdit.LastName}): ");
            string newLastName = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newLastName)) newLastName = clientToEdit.LastName;

            Write($"Nueva Identificación ({clientToEdit.Identification}): ");
            string newIdentification = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newIdentification)) newIdentification = clientToEdit.Identification;

            Write($"Nuevo Teléfono ({clientToEdit.Phone}): ");
            string newPhone = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newPhone)) newPhone = clientToEdit.Phone;

            Write($"Nuevo Email ({clientToEdit.Email}): ");
            string newEmail = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newEmail)) newEmail = clientToEdit.Email;
            
            // Llamar al servicio con todos los datos (actualizados o los originales si se dejaron vacíos)
            _clientService.EditClient(id, newFirstName, newLastName, newIdentification, newPhone, newEmail);
        }
        else
        {
            WriteLine("ID inválido.");
        }
        Pausar();
    }
    
    // *** CLIENTE: Incluye la visualización de datos y confirmación de eliminación ***
    private static void DeleteClientHandler()
    {
        Write("ID del cliente a eliminar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            var clientToDelete = _clientService.GetClientById(id);
            
            if (clientToDelete == null)
            {
                WriteLine($"Cliente ID {id} no encontrado.");
                Pausar();
                return;
            }
            
            // MOSTRAR INFORMACIÓN PARA CONFIRMACIÓN
            WriteLine("\n--- DATOS DEL CLIENTE A ELIMINAR ---");
            WriteLine($"ID: {clientToDelete.ClientId}, Nombre: {clientToDelete.FirstName} {clientToDelete.LastName}");
            WriteLine($"Identificación: {clientToDelete.Identification}");
            WriteLine($"Email: {clientToDelete.Email}");
            WriteLine("----------------------------------");

            Write($"¿Está seguro que desea eliminar al cliente ID {id}? Esta acción es irreversible (S/N): ");
            string confirmation = ReadLine()?.ToUpper() ?? string.Empty;

            if (confirmation == "S")
            {
                _clientService.DeleteClient(id);
            }
            else
            {
                WriteLine("Eliminación cancelada.");
            }
        }
        else
        {
            WriteLine("ID inválido.");
        }
        Pausar();
    }
    
    // --- GESTIÓN DE MASCOTAS ---
    public static void ManagePetsMenu()
    {
        bool back = false;
        while (!back)
        {
            Clear();
            WriteLine("--- GESTIÓN DE MASCOTAS ---");
            WriteLine("1. Registrar Mascota");
            WriteLine("2. Listar Mascotas");
            WriteLine("3. Editar Mascota");
            WriteLine("4. Eliminar Mascota");
            WriteLine("0. Volver al Menú Principal");
            Write("Opción: ");

            string? option = ReadLine();
            WriteLine("---------------------------------");

            switch (option)
            {
                case "1":
                    RegisterPetHandler();
                    break;
                case "2":
                    ListPetsHandler();
                    break;
                case "3":
                    EditPetHandler(); // YA INCLUYE MOSTRAR INFO
                    break;
                case "4":
                    DeletePetHandler(); // YA INCLUYE MOSTRAR INFO Y CONFIRMACIÓN
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    WriteLine("Opción no válida.");
                    Pausar();
                    break;
            }
        }
    }
    
    private static void RegisterPetHandler()
    {
        Write("ID del Dueño (Cliente): ");
        if (int.TryParse(ReadLine(), out int clientId))
        {
            Write("Nombre de la Mascota: ");
            string name = ReadLine() ?? string.Empty; 
            Write("Especie (Ej: Perro, Gato): ");
            string species = ReadLine() ?? string.Empty; 
            Write("Raza: ");
            string breed = ReadLine() ?? string.Empty; 

            var newPet = new Pet
            {
                Name = name,
                Species = species,
                Breed = breed,
                Client = null
            };
            _petService.RegisterPet(newPet, clientId);
        }
        else
        {
            WriteLine("ID de Cliente inválido.");
        }
        Pausar();
    }

    private static void ListPetsHandler()
    {
        WriteLine("¿Desea listar todas las mascotas (1) o filtrar por especie (2)?");
        Write("Opción: ");
        string subOption = ReadLine() ?? string.Empty; 
        List<Pet> pets;
        
        if (subOption == "2")
        {
            Write("Ingrese la especie a filtrar: ");
            string species = ReadLine() ?? string.Empty; 
            pets = _petService.ListPets(species); 
        }
        else
        {
            pets = _petService.ListPets(); 
        }
        
        if (pets.Any())
        {
            WriteLine("--- LISTADO DE MASCOTAS ---");
            WriteLine($"{"ID",-5} {"NOMBRE",-15} {"ESPECIE",-10} {"RAZA",-15} {"DUEÑO"}");
            WriteLine(new string('-', 70));
            foreach (var m in pets)
            {
                // FIX: Limpiar la expresión de concatenación/nullabilidad para evitar advertencias y mejorar la claridad.
                string ownerName = (m.Client != null) ? $"{m.Client.FirstName} {m.Client.LastName}" : "N/A";
                WriteLine($"{m.PetId,-5} {m.Name,-15} {m.Species,-10} {m.Breed,-15} {ownerName}");
            }
        }
        else
        {
            WriteLine("No hay mascotas registradas o especie no encontrada.");
        }
        Pausar();
    }

    // *** MASCOTA: Incluye la visualización de datos actuales antes de la edición ***
    private static void EditPetHandler()
    {
        Write("ID de la mascota a editar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            var petToEdit = _petService.GetPetById(id);
            
            if (petToEdit == null)
            {
                WriteLine($"Mascota ID {id} no encontrada.");
                Pausar();
                return;
            }

            // MOSTRAR INFORMACIÓN ACTUAL
            WriteLine("\n--- DATOS ACTUALES DE LA MASCOTA ---");
            WriteLine($"Nombre Actual: {petToEdit.Name}");
            WriteLine($"Especie: {petToEdit.Species}");
            WriteLine($"Raza Actual: {petToEdit.Breed}");
            // FIX: Limpiar la expresión de concatenación/nullabilidad para evitar advertencias y mejorar la claridad.
            string ownerDisplay = (petToEdit.Client != null) ? $"{petToEdit.Client.FirstName} {petToEdit.Client.LastName}" : "N/A";
            WriteLine($"Dueño: {ownerDisplay} (ID: {petToEdit.ClientId})");
            WriteLine("----------------------------------");

            WriteLine("\nIngrese nuevos valores (deje vacío y presione Enter para mantener el valor actual):");

            Write($"Nuevo Nombre ({petToEdit.Name}): ");
            string newName = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newName)) newName = petToEdit.Name;
            
            Write($"Nueva Raza ({petToEdit.Breed}): ");
            string newBreed = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newBreed)) newBreed = petToEdit.Breed;
            
            _petService.EditPet(id, newName, newBreed);
        }
        else
        {
            WriteLine("ID inválido.");
        }
        Pausar();
    }
    
    // *** MASCOTA: Incluye la visualización de datos y confirmación de eliminación ***
    private static void DeletePetHandler()
    {
        Write("ID de la mascota a eliminar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            var petToDelete = _petService.GetPetById(id);
            
            if (petToDelete == null)
            {
                WriteLine($"Mascota ID {id} no encontrada.");
                Pausar();
                return;
            }

            // MOSTRAR INFORMACIÓN PARA CONFIRMACIÓN
            WriteLine("\n--- DATOS DE LA MASCOTA A ELIMINAR ---");
            WriteLine($"ID: {petToDelete.PetId}, Nombre: {petToDelete.Name}");
            WriteLine($"Especie: {petToDelete.Species}, Raza: {petToDelete.Breed}");
            // FIX: Limpiar la expresión de concatenación/nullabilidad para evitar advertencias y mejorar la claridad.
            string ownerDisplay = (petToDelete.Client != null) ? $"{petToDelete.Client.FirstName} {petToDelete.Client.LastName}" : "N/A";
            WriteLine($"Dueño: {ownerDisplay} (ID: {petToDelete.ClientId})");
            WriteLine("----------------------------------");

            Write($"¿Está seguro que desea eliminar a la mascota ID {id}? (S/N): ");
            string confirmation = ReadLine()?.ToUpper() ?? string.Empty;

            if (confirmation == "S")
            {
                _petService.DeletePet(id);
            }
            else
            {
                WriteLine("Eliminación cancelada.");
            }
        }
        else
        {
            WriteLine("ID inválido.");
        }
        Pausar();
    }


    // --- GESTIÓN DE VETERINARIOS ---
    public static void ManageVeterinariansMenu()
    {
        bool back = false;
        while (!back)
        {
            Clear();
            WriteLine("--- GESTIÓN DE VETERINARIOS ---");
            WriteLine("1. Registrar Veterinario");
            WriteLine("2. Listar Veterinarios");
            WriteLine("3. Editar Veterinario");
            WriteLine("4. Eliminar Veterinario");
            WriteLine("0. Volver al Menú Principal");
            Write("Opción: ");

            string? option = ReadLine();
            WriteLine("---------------------------------");

            switch (option)
            {
                case "1":
                    RegisterVeterinarianHandler();
                    break;
                case "2":
                    ListVeterinariansHandler();
                    break;
                case "3":
                    EditVeterinarianHandler(); // YA INCLUYE MOSTRAR INFO
                    break;
                case "4":
                    DeleteVeterinarianHandler(); // YA INCLUYE MOSTRAR INFO Y CONFIRMACIÓN
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    WriteLine("Opción no válida.");
                    Pausar();
                    break;
            }
        }
    }
    
    private static void RegisterVeterinarianHandler()
    {
        Write("Nombre del Veterinario: ");
        string name = ReadLine() ?? string.Empty; 
        Write("Especialidad: ");
        string specialty = ReadLine() ?? string.Empty; 

        var newVeterinarian = new Veterinarian { Name = name, Specialty = specialty };
        _veterinarianService.RegisterVeterinarian(newVeterinarian);
        Pausar();
    }

    public static void ListVeterinariansHandler()
    {
        var veterinarians = _veterinarianService.ListVeterinarians();
        if (veterinarians.Any())
        {
            WriteLine("--- LISTADO DE VETERINARIOS ---");
            WriteLine($"{"ID",-5} {"NOMBRE",-30} {"ESPECIALIDAD"}");
            WriteLine(new string('-', 60));
            foreach (var v in veterinarians)
            {
                WriteLine($"{v.VeterinarianId,-5} {v.Name,-30} {v.Specialty}");
            }
        }
        else
        {
            WriteLine("No hay veterinarios registrados.");
        }
        Pausar();
    }

    // *** VETERINARIO: Incluye la visualización de datos actuales antes de la edición ***
    private static void EditVeterinarianHandler()
    {
        Write("ID del veterinario a editar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            var vetToEdit = _veterinarianService.GetVeterinarianById(id);

            if (vetToEdit == null)
            {
                WriteLine($"Veterinario ID {id} no encontrado.");
                Pausar();
                return;
            }

            // MOSTRAR INFORMACIÓN ACTUAL
            WriteLine("\n--- DATOS ACTUALES DEL VETERINARIO ---");
            WriteLine($"Nombre Actual: {vetToEdit.Name}");
            WriteLine($"Especialidad Actual: {vetToEdit.Specialty}");
            WriteLine("--------------------------------------");
            
            WriteLine("\nIngrese nuevos valores (deje vacío y presione Enter para mantener el valor actual):");

            Write($"Nuevo Nombre ({vetToEdit.Name}): ");
            string newName = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newName)) newName = vetToEdit.Name;

            Write($"Nueva Especialidad ({vetToEdit.Specialty}): ");
            string newSpecialty = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newSpecialty)) newSpecialty = vetToEdit.Specialty;
            
            _veterinarianService.EditVeterinarian(id, newName, newSpecialty);
        }
        else
        {
            WriteLine("ID inválido.");
        }
        Pausar();
    }
    
    // *** VETERINARIO: Incluye la visualización de datos y confirmación de eliminación ***
    private static void DeleteVeterinarianHandler()
    {
        Write("ID del veterinario a eliminar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            var vetToDelete = _veterinarianService.GetVeterinarianById(id);

            if (vetToDelete == null)
            {
                WriteLine($"Veterinario ID {id} no encontrado.");
                Pausar();
                return;
            }
            
            // MOSTRAR INFORMACIÓN PARA CONFIRMACIÓN
            WriteLine("\n--- DATOS DEL VETERINARIO A ELIMINAR ---");
            WriteLine($"ID: {vetToDelete.VeterinarianId}, Nombre: {vetToDelete.Name}");
            WriteLine($"Especialidad: {vetToDelete.Specialty}");
            WriteLine("--------------------------------------");

            Write($"¿Está seguro que desea eliminar al veterinario ID {id}? (S/N): ");
            string confirmation = ReadLine()?.ToUpper() ?? string.Empty;

            if (confirmation == "S")
            {
                _veterinarianService.DeleteVeterinarian(id);
            }
            else
            {
                WriteLine("Eliminación cancelada.");
            }
        }
        else
        {
            WriteLine("ID inválido.");
        }
        Pausar();
    }


    // --- GESTIÓN DE CITAS MÉDICAS ---
    public static void ManageAppointmentsMenu()
    {
        bool back = false;
        while (!back)
        {
            Clear();
            WriteLine("--- GESTIÓN DE CITAS MÉDICAS ---");
            WriteLine("1. Registrar Cita Médica");
            WriteLine("2. Listar Citas Médicas"); // Corregido: Anteriormente listaba Veterinarios
            WriteLine("3. Editar Cita Médica (Diagnóstico)");
            WriteLine("4. Eliminar Cita Médica");
            WriteLine("0. Volver al Menú Principal");
            Write("Opción: ");

            string? option = ReadLine();
            WriteLine("---------------------------------");

            switch (option)
            {
                case "1":
                    RegisterAppointmentHandler();
                    break;
                case "2":
                    ListAppointmentsHandler(); // <--- CORREGIDO: Llama a ListAppointmentsHandler
                    break;
                case "3":
                    EditAppointmentHandler(); // YA INCLUYE MOSTRAR INFO
                    break;
                case "4":
                    DeleteAppointmentHandler(); // YA INCLUYE MOSTRAR INFO Y CONFIRMACIÓN
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    WriteLine("Opción no válida.");
                    Pausar();
                    break;
            }
        }
    }
    
    public static void RegisterAppointmentHandler()
    {
        Write("ID de la Mascota: ");
        if (!int.TryParse(ReadLine(), out int petId))
        {
            WriteLine("ID de Mascota inválido.");
            Pausar();
            return;
        }

        // --- INICIO DE CORRECCIÓN: VALIDACIÓN DE FK ---
        // 1. Validar que la Mascota exista
        if (_petService.GetPetById(petId) == null)
        {
            WriteLine($"ERROR: Mascota con ID {petId} no encontrada. No se puede crear la cita.");
            Pausar();
            return;
        }
        // --- FIN DE CORRECCIÓN ---


        Write("ID del Veterinario: ");
        if (!int.TryParse(ReadLine(), out int veterinarianId))
        {
            WriteLine("ID de Veterinario inválido.");
            Pausar();
            return;
        }

        // --- INICIO DE CORRECCIÓN: VALIDACIÓN DE FK ---
        // 2. Validar que el Veterinario exista
        if (_veterinarianService.GetVeterinarianById(veterinarianId) == null)
        {
            WriteLine($"ERROR: Veterinario con ID {veterinarianId} no encontrado. No se puede crear la cita.");
            Pausar();
            return;
        }
        // --- FIN DE CORRECCIÓN ---


        Write("Diagnóstico: ");
        string diagnosis = ReadLine() ?? string.Empty;

        // Usamos la fecha y hora actuales
        _appointmentService.RegisterAppointment(DateTime.Now, diagnosis, petId, veterinarianId);
        Pausar();
    }
    
    // Función para listar Citas Médicas
    public static void ListAppointmentsHandler()
    {
        var appointments = _appointmentService.ListAppointments();
        if (appointments.Any())
        {
            WriteLine("--- LISTADO DE CITAS ---");
            WriteLine($"{"ID",-5} {"FECHA",-12} {"MASCOTA ID",-10} {"VETERINARIO ID",-15} {"DIAGNÓSTICO (RESUMEN)"}");
            WriteLine(new string('-', 75));
            foreach (var a in appointments)
            {
                 // Muestra detalles de la relación Mascota y Veterinario
                 string diagnosisSummary = a.Diagnosis != null && a.Diagnosis.Length > 20 ? a.Diagnosis.Substring(0, 17) + "..." : a.Diagnosis ?? "N/A";
                 WriteLine($"{a.AppointmentId,-5} {a.Date:dd/MM/yyyy,-12} {a.PetId,-10} {a.VeterinarianId,-15} {diagnosisSummary}");
            }
        }
        else
        {
            WriteLine("No hay citas registradas.");
        }
        Pausar();
    }

    // *** CITA: Incluye la visualización de datos actuales antes de la edición ***
    private static void EditAppointmentHandler()
    {
        Write("ID de la cita a editar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            var appointmentToEdit = _appointmentService.GetAppointmentById(id);

            if (appointmentToEdit == null)
            {
                WriteLine($"Cita ID {id} no encontrada.");
                Pausar();
                return;
            }

            // MOSTRAR INFORMACIÓN ACTUAL
            WriteLine("\n--- DATOS ACTUALES DE LA CITA ---");
            WriteLine($"Fecha: {appointmentToEdit.Date:dd/MM/yyyy}");
            WriteLine($"Mascota: {appointmentToEdit.Pet?.Name ?? "N/A"} (ID: {appointmentToEdit.PetId})");
            // FIX: Limpiar la expresión de concatenación/nullabilidad para evitar advertencias y mejorar la claridad.
            string vetDisplay = (appointmentToEdit.Veterinarian != null) ? appointmentToEdit.Veterinarian.Name : "N/A";
            WriteLine($"Veterinario: {vetDisplay} (ID: {appointmentToEdit.VeterinarianId})");
            WriteLine($"Diagnóstico Actual: {appointmentToEdit.Diagnosis}");
            WriteLine("---------------------------------");
            
            WriteLine("\nIngrese el nuevo Diagnóstico (deje vacío y presione Enter para mantener el valor actual):");
            Write($"Nuevo Diagnóstico ({appointmentToEdit.Diagnosis}): ");
            string newDiagnosis = ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newDiagnosis)) newDiagnosis = appointmentToEdit.Diagnosis;
                
            _appointmentService.EditAppointment(id, newDiagnosis);
        }
        else
        {
            WriteLine("ID inválido.");
        }
        Pausar();
    }
    
    // *** CITA: Incluye la visualización de datos y confirmación de eliminación ***
    private static void DeleteAppointmentHandler()
    {
        Write("ID de la cita a eliminar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            var appointmentToDelete = _appointmentService.GetAppointmentById(id);

            if (appointmentToDelete == null)
            {
                WriteLine($"Cita ID {id} no encontrada.");
                Pausar();
                return;
            }

            // MOSTRAR INFORMACIÓN PARA CONFIRMACIÓN
            WriteLine("\n--- DATOS DE LA CITA A ELIMINAR ---");
            WriteLine($"ID: {appointmentToDelete.AppointmentId}");
            WriteLine($"Fecha: {appointmentToDelete.Date:dd/MM/yyyy}");
            WriteLine($"Mascota: {appointmentToDelete.Pet?.Name ?? "N/A"} (ID: {appointmentToDelete.PetId})");
            // FIX: Limpiar la expresión de concatenación/nullabilidad para evitar advertencias y mejorar la claridad.
            string vetDisplay = (appointmentToDelete.Veterinarian != null) ? appointmentToDelete.Veterinarian.Name : "N/A";
            WriteLine($"Veterinario: {vetDisplay} (ID: {appointmentToDelete.VeterinarianId})");
            WriteLine($"Diagnóstico: {appointmentToDelete.Diagnosis}");
            WriteLine("---------------------------------");

            Write($"¿Está seguro que desea eliminar la cita ID {id}? (S/N): ");
            string confirmation = ReadLine()?.ToUpper() ?? string.Empty;

            if (confirmation == "S")
            {
                _appointmentService.DeleteAppointment(id);
            }
            else
            {
                WriteLine("Eliminación cancelada.");
            }
        }
        else
        {
            WriteLine("ID inválido.");
        }
        Pausar();
    }


    // --- HISTORIAL MÉDICO ---
    public static void MedicalHistoryMenu()
    {
        Clear();
        WriteLine("--- HISTORIAL MÉDICO ---");
        Write("Ingrese el ID de la mascota para ver el historial: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            var history = _appointmentService.GetMedicalHistory(id);
            if (history.Any())
            {
                WriteLine($"--- HISTORIAL DE MASCOTA ID {id} ---");
                foreach (var a in history)
                {
                    WriteLine($"Fecha: {a.Date:dd/MM/yyyy}, Diagnóstico: {a.Diagnosis}, Atendido por: {a.Veterinarian?.Name ?? "N/A"}");
                }
            }
            else
            {
                WriteLine($"No hay historial para la mascota ID {id}.");
            }
        }
        else
        {
            WriteLine("ID inválido.");
        }
        Pausar();
    }
    
    // --- CONSULTAS AVANZADAS ---
    public static void AdvancedQueriesMenu()
    {
        bool back = false;
        while (!back)
        {
            Clear();
            WriteLine("--- CONSULTAS AVANZADAS (LINQ sobre EF) ---");
            WriteLine("1. Consultar todas las mascotas de un cliente.");
            WriteLine("2. Consultar el veterinario con más citas realizadas.");
            WriteLine("3. Consultar la especie de mascota más atendida en la clínica.");
            WriteLine("4. Consultar el cliente con más mascotas registradas.");
            WriteLine("0. Volver al Menú Principal");
            Write("Opción: ");
            
            string? option = ReadLine();
            WriteLine("---------------------------------");
            
            switch (option)
            {
                case "1":
                    ConsultarMascotasDeClienteHandler();
                    break;
                case "2":
                    GetBusiestVeterinarianHandler();
                    break;
                case "3":
                    GetMostAttendedSpeciesHandler();
                    break;
                case "4":
                    GetClientWithMostPetsHandler();
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    WriteLine("Opción no válida.");
                    break;
            }
            if (!back) Pausar();
        }
    }

    private static void ConsultarMascotasDeClienteHandler()
    {
        Write("ID del cliente a consultar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            // Se asume que _appointmentService.GetPetsByClient(id) retorna una lista de Pet
            var pets = _appointmentService.GetPetsByClient(id); 
            if (pets.Any())
            {
                WriteLine($"Mascotas del cliente ID {id}:");
                foreach (var m in pets)
                {
                    WriteLine($"- ID: {m.PetId}, Nombre: {m.Name}, Especie: {m.Species}");
                }
            }
            else
            {
                WriteLine($"El cliente ID {id} no tiene mascotas registradas.");
            }
        }
        else
        {
            WriteLine("ID inválido.");
        }
    }

    private static void GetBusiestVeterinarianHandler()
    {
        var vet = _appointmentService.GetBusiestVeterinarian();
        if (vet != null)
        {
            WriteLine($"El veterinario con más citas es: {vet.Name} ({vet.Specialty}).");
        }
        else
        {
            WriteLine("No hay citas registradas para realizar esta consulta.");
        }
    }
    
    private static void GetMostAttendedSpeciesHandler()
    {
        string? species = _appointmentService.GetMostAttendedSpecies();
        if (!string.IsNullOrEmpty(species))
        {
            WriteLine($"La especie de mascota más atendida es: {species}.");
        }
        else
        {
            WriteLine("No hay datos de citas suficientes.");
        }
    }

    private static void GetClientWithMostPetsHandler()
    {
        var client = _appointmentService.GetClientWithMostPets();
        if (client != null)
        {
            WriteLine($"El cliente con más mascotas registradas es: {client.FirstName} (ID: {client.ClientId}).");
        }
        else
        {
            WriteLine("No hay mascotas registradas.");
        }
    }
}
