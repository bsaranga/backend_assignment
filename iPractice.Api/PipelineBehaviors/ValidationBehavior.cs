using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using iPractice.Api.Validation;
using MediatR;

namespace iPractice.Api.PipelineBehaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private IEnumerable<IValidator<TRequest>> validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validator = validators.SingleOrDefault(v => v.GetType().GenericTypeArguments == typeof(TRequest).GenericTypeArguments);

        if (validator != null)
        {
            await validator.ValidateAsync(request, cancellationToken);
        }

        return await next();
    }
}
