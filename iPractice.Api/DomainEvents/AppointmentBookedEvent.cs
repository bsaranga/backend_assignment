using System.Threading;
using System.Threading.Tasks;
using iPractice.Api.Models.Clients;
using iPractice.Api.Services;
using MediatR;

namespace iPractice.Api.DomainEvents;

public record AppointmentBookedEvent(Appointment Appointment, string ClientName) : INotification;

public class AppointmentBookedEventHandler(EmailService emailService) : INotificationHandler<AppointmentBookedEvent>
{
    public async Task Handle(AppointmentBookedEvent notification, CancellationToken cancellationToken)
    {
        await emailService.SendBookingConfirmationEmail(notification.ClientName, notification.Appointment.From.ToString());
    }
}