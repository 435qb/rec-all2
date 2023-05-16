using System.Collections;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using RecAll.Core.List.Domain.Exceptions;
using RecAll.Infrastructure;

namespace RecAll.Core.List.Api.Application.Behaviors;

public class 
    ListDomainExceptionHandler<TRequest> 
        : IRequestExceptionHandler<TRequest, ServiceResult, ListDomainException> where TRequest : IRequest<ServiceResult>
{
    private readonly ILogger<LoggingBehavior<TRequest, ServiceResult>> _logger;

    public ListDomainExceptionHandler(
        ILogger<LoggingBehavior<TRequest, ServiceResult>> logger)
    {
        _logger = logger;
    }
    public Task Handle(TRequest request, ListDomainException exception, RequestExceptionHandlerState<ServiceResult> state, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "----- Handling exception from validating command {CommandName} ({@Command})",
            request.GetType().Name, request);
        
        IEnumerable<string> messages;
        if (exception.InnerException is ValidationException validationException)
        {
            var list = validationException.Errors;
            messages = list.Select(p => p.ToString());
        }
        else
        {
            messages = new[] { exception.Message };
        }
        state.SetHandled(ServiceResult.CreateExceptionResult(messages));
        _logger.LogInformation(
            "----- Exception for {CommandName} handled ({@Message})",
            request.GetType().Name, messages);
        return Task.CompletedTask;
    }

}