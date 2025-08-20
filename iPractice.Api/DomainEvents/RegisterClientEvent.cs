using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using iPractice.Api.Services;
using MediatR;

namespace iPractice.Api.DomainEvents;

public record RegisterClientEvent(string ClientName) : INotification;

public class RegisterClientEventHandler(EmailService emailService) : INotificationHandler<RegisterClientEvent>
{
    public async Task Handle(RegisterClientEvent notification, CancellationToken cancellationToken)
    {
        await emailService.SendWelcomeEmail(notification.ClientName, "welcome@ipractice.com");
    }
}
