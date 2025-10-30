using Microsoft.Extensions.Logging;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Serilog.Context;


namespace Pento.Application.Abstractions.Behaviors;

internal static class LoggingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;
            try
            {
                logger.LogInformation("Processing command {Command}", commandName);

                Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Completed command {Command}", commandName);
                }
                else
                {
                    using (LogContext.PushProperty("Error", result.Error, true))
                    {
                        logger.LogError("Command {CommandName} processed with error", commandName);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Command {CommandName} processing failed", commandName);

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
            string commandName = typeof(TCommand).Name;
            try
            {
                logger.LogInformation("Processing command {Command}", commandName);

                Result result = await innerHandler.Handle(command, cancellationToken);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Completed command {Command}", commandName);
                }
                else
                {
                    using (LogContext.PushProperty("Error", result.Error, true))
                    {
                        logger.LogError("Command {CommandName} processed with error", commandName);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Command {CommandName} processing failed", commandName);

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
            string queryName = typeof(TQuery).Name;
            try
            {
                logger.LogInformation("Processing query {Query}", queryName);

                Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Completed query {Query}", queryName);
                }
                else
                {
                    using (LogContext.PushProperty("Error", result.Error, true))
                    {
                        logger.LogError("Query {QueryName} processed with error", queryName);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Query {QueryName} processing failed", queryName);

                throw new PentoException(typeof(TQuery).Name, innerException: ex);
            }
        }
        
    }
}
