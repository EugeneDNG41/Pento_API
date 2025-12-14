using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Infrastructure.Utility.Outbox;
using Quartz;
namespace Pento.Infrastructure.External.Quartz;
#pragma warning disable S125
[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessagesCleanupJob(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<OutboxMessage> outboxMessageRepository,
    IUnitOfWork unitOfWork
    ) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var oldMessages = (await outboxMessageRepository
            .FindAsync(
                m => m.ProcessedOnUtc != null &&
                m.ProcessedOnUtc <= dateTimeProvider.UtcNow.AddDays(-7), context.CancellationToken))
            .ToList();
        outboxMessageRepository.RemoveRange(oldMessages);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
