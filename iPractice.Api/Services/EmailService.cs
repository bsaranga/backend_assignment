using System;
using System.Threading.Tasks;

namespace iPractice.Api.Services;

public class EmailService
{
    public async Task SendWelcomeEmail(string name, string email)
    {
        Console.WriteLine($"Welcome email sent to {name} at {email}");
        await Task.Delay(100); // Simulate sending
    }

    public async Task SendBookingConfirmationEmail(string clientName, string appointmentTime)
    {
        Console.WriteLine($"Booking confirmation sent to {clientName} for {appointmentTime}");
        await Task.Delay(100);
    }
}