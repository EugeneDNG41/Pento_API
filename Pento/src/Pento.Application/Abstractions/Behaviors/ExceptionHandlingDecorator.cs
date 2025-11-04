using JasperFx;
using Microsoft.Extensions.Logging;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using ConcurrencyException = JasperFx.ConcurrencyException;

namespace Pento.Application.Abstractions.Behaviors;

internal sealed class ExceptionHandlingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return await innerHandler.Handle(command, cancellationToken);
            }
            catch (ConcurrencyException)
            {
                return Result.Failure<TResponse>(Error.Conflict(
                    "Version Mismatch",
                    "The entity has been modified by another process"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception for {CommandName}", typeof(TCommand).Name);
                throw new PentoException(typeof(TCommand).Name, innerException: ex);
            }
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return await innerHandler.Handle(command, cancellationToken);
            }
            catch (ConcurrencyException)
            {
                return Result.Failure(Error.Conflict(
                    "Version Mismatch",
                    "The entity has been modified by another process"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception for {CommandName}", typeof(TCommand).Name);
                throw new PentoException(typeof(TCommand).Name, innerException: ex);
            }
        }
    }
    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            try
            {
                return await innerHandler.Handle(query, cancellationToken);
            }
            catch (ConcurrencyException)
            {
                return Result.Failure<TResponse>(Error.Conflict(
                    "Version Mismatch",
                    "The entity has been modified by another process"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception for {QueryName}", typeof(TQuery).Name);
                throw new PentoException(typeof(TQuery).Name, innerException: ex);
            }
        }
    }
}
