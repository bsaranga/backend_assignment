using System;
using System.Threading;
using System.Threading.Tasks;
using iPractice.Api.Repositories;
using iPractice.Api.UseCases.Clients;

namespace iPractice.Api.Validation;

public class BookAppointmentValidator : IValidator<BookAppointmentCommand>
{
    private readonly IClientSqlRepository clientSqlRepository;

    public BookAppointmentValidator(IClientSqlRepository clientSqlRepository)
    {
        this.clientSqlRepository = clientSqlRepository;
    }

    public async Task ValidateAsync(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        var client = await clientSqlRepository.GetClientByIdAsync(request.ClientId, cancellationToken);

        if (client == null)
        {
            throw new Exception($"Client with ID {request.ClientId} not found");
        }
    }
}
