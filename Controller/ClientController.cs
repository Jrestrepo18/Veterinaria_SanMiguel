using System.Collections.Generic;
using System.Linq;
using veterinaria_san_miguel.Data;
using veterinaria_san_miguel.Modals; 
using static System.Console;

public class ClientService
{
    public ClientService() {}

    // 1. Register Client (Actualizado para todos los campos)
    public void RegisterClient(Client client)
    {
        using (var context = new MysqlDbContext())
        {
            // Asignar las fechas de registro justo antes de guardar (si no se han asignado antes)
            client.RegistrationDate = DateTime.Now;
            client.RegisterDate = DateTime.Now;
            
            context.Clients.Add(client);
            context.SaveChanges();
            WriteLine($"Cliente {client.FirstName} {client.LastName} registrado con ID: {client.ClientId}");
        }
    }

    // 1.5 Get Client by ID
    public Client? GetClientById(int clientId)
    {
        using (var context = new MysqlDbContext())
        {
            return context.Clients.Find(clientId);
        }
    }

    // 2. List Clients
    public List<Client> ListClients()
    {
        using (var context = new MysqlDbContext())
        {
            return context.Clients.ToList();
        }
    }

    // 3. Edit Client (Actualizado para todos los campos relevantes)
    public void EditClient(int clientId, string newFirstName, string newLastName, string newIdentification, string newPhone, string newEmail)
    {
        using (var context = new MysqlDbContext())
        {
            var client = context.Clients.Find(clientId); 
            if (client != null)
            {
                client.FirstName = newFirstName;
                client.LastName = newLastName;
                client.Identification = newIdentification;
                client.Phone = newPhone;
                client.Email = newEmail;
                
                // Las fechas de registro generalmente no se editan, por lo que se mantienen.
                context.SaveChanges();
                WriteLine($"Cliente ID {clientId} actualizado.");
            }
            else
            {
                WriteLine($"Cliente ID {clientId} no encontrado.");
            }
        }
    }

    // 4. Delete Client 
    public void DeleteClient(int clientId)
    {
        using (var context = new MysqlDbContext())
        {
            var client = context.Clients.Find(clientId);
            if (client != null)
            {
                context.Clients.Remove(client);
                context.SaveChanges();
                WriteLine($"Cliente ID {clientId} eliminado.");
            }
            else
            {
                WriteLine($"Cliente ID {clientId} not found.");
            }
        }
    }
}