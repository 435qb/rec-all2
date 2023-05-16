using System.Reflection;
using Autofac;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using RecAll.Core.List.Api.Application.Behaviors;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Api.Application.Validators;
using RecAll.Core.List.Domain.Exceptions;
using RecAll.Infrastructure;
using Module = Autofac.Module;

namespace RecAll.Core.List.Api.Infrastructure.AutofacModules;

public class MediatorModule : Module {
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(ListDomainExceptionHandler<>)).As(typeof(IRequestExceptionHandler<,,>));
        
        builder.RegisterGeneric(typeof(RequestExceptionProcessorBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));
        
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(typeof(CreateListCommand).GetTypeInfo()
            .Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));

        builder.RegisterAssemblyTypes(typeof(CreateListCommandValidator).GetTypeInfo()
            .Assembly).AsClosedTypesOf(typeof(IValidator<>));
        
        builder.Register<ServiceFactory>(context => {
            var componentContext = context.Resolve<IComponentContext>();
            return t => componentContext.TryResolve(t, out var o) ? o : null;
        });

        builder.RegisterGeneric(typeof(LoggingBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));

        builder.RegisterGeneric(typeof(ValidatorBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));

        builder.RegisterGeneric(typeof(TransactionBehaviour<,>))
            .As(typeof(IPipelineBehavior<,>));
    }
}