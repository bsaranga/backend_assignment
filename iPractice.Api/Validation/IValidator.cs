using System;
using System.Threading;
using System.Threading.Tasks;

namespace iPractice.Api.Validation;

public interface IValidator<TRequest>
{
    Task ValidateAsync(TRequest request, CancellationToken cancellationToken);
}
