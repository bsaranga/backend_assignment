using System.Threading;
using System.Threading.Tasks;
using iPractice.Api.Validation;
using MediatR;

namespace iPractice.Api.PipelineBehaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IValidator<TRequest> validator;

    public ValidationBehavior(IValidator<TRequest> validator)
    {
        this.validator = validator;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await validator.ValidateAsync(request, cancellationToken);
        return await next();
    }
}
