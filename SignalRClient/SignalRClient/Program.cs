// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // Define the Hub connection
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5122/chatHub") // Change URL based on your server
            .WithAutomaticReconnect()
            .Build();

        // Listen for messages from the server
        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Console.WriteLine($"{user}: {message}");
        });

        try
        {
            // Start the connection
            await connection.StartAsync();
            Console.WriteLine("Connected to ChatHub!");

            while (true)
            {
                Console.Write("Enter your name: ");
                string user = Console.ReadLine();

                Console.Write("Enter message: ");
                string message = Console.ReadLine();

                if (string.IsNullOrEmpty(message)) break; // Exit on empty message

                // Send message to the hub
                await connection.InvokeAsync("SendMessage", user, message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting: {ex.Message}");
        }

        Console.WriteLine("Disconnected. Press any key to exit.");
        Console.ReadKey();
    }
}

