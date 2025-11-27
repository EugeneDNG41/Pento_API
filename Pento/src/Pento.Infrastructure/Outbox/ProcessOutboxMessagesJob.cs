using System.Data;
using System.Reflection.Metadata;
using Dapper;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Quartz;

namespace Pento.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessagesJob : IJob
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        ISqlConnectionFactory sqlConnectionFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeProvider dateTimeProvider,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _serviceScopeFactory = serviceScopeFactory;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _outboxOptions = outboxOptions.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Beginning to process outbox messages");

        using IDbConnection connection = await _sqlConnectionFactory.OpenConnectionAsync(context.CancellationToken);
        using IDbTransaction transaction = connection.BeginTransaction();

        IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            Exception? exception = null;
            var eventType = Type.GetType(outboxMessage.Type);

            if (eventType is null)
            {
                // log & skip or throw
                _logger.LogError("Could not resolve type {Type} from Outbox Message {Id}", outboxMessage.Type, outboxMessage.Id);
                continue;
            }
            try
            {
                var domainEvent = (IDomainEvent)JsonConvert.DeserializeObject(
                    outboxMessage.Content,
                    type: eventType,
                    JsonSerializerSettings)!;
                using IServiceScope scope = _serviceScopeFactory.CreateScope();

                IEnumerable<IDomainEventHandler> handlers = DomainEventHandlersFactory.GetHandlers(
                    domainEvent.GetType(),
                    scope.ServiceProvider,
                    typeof(IDomainEventHandler).Assembly);

                foreach (IDomainEventHandler domainEventHandler in handlers)
                {
                    await domainEventHandler.Handle(domainEvent, context.CancellationToken);
                }
            }
            catch (Exception caughtException)
            {
                _logger.LogError(
                    caughtException,
                    "Exception while processing outbox message {MessageId}",
                    outboxMessage.Id);

                exception = caughtException;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();

        _logger.LogInformation("Completed processing outbox messages");
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql = $"""
                      SELECT id, content, type
                      FROM outbox_messages
                      WHERE processed_on_utc IS NULL
                      ORDER BY occurred_on_utc
                      LIMIT {_outboxOptions.BatchSize}
                      FOR UPDATE
                      """;

        IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
            sql,
            transaction: transaction);

        return outboxMessages.ToList();
    }

    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql = @"
            UPDATE outbox_messages
            SET processed_on_utc = @ProcessedOnUtc,
                error = @Error
            WHERE id = @Id";

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = _dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            },
            transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content, string Type);
}
