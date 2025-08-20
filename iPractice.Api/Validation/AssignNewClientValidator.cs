using System;
using System.Threading;
using System.Threading.Tasks;
using iPractice.Api.Repositories;
using iPractice.Api.UseCases.Psychologists;

namespace iPractice.Api.Validation;

public class AssignNewClientValidator : IValidator<AssignNewClientCommand>
{
    private readonly IClientSqlRepository clientSqlRepository;

    public AssignNewClientValidator(IClientSqlRepository clientSqlRepository)
    {
        this.clientSqlRepository = clientSqlRepository;
    }

    public async Task ValidateAsync(AssignNewClientCommand request, CancellationToken cancellationToken)
    {
        var client = await clientSqlRepository.GetClientByIdAsync(request.ClientId, cancellationToken);

        if (client == null)
            throw new Exception($"Client with ID={request.ClientId} does not exist");
    }
}
