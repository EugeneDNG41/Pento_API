using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Authentication;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.StorageItems;
using Pento.Domain.StorageItems.Events;
using Pento.Domain.Users;
using Pento.Infrastructure;
using Pento.Infrastructure.Repositories;

namespace Pento.Infrastructure.Repositories;

internal sealed class StorageItemRepository(IDocumentStore store, IUserContext userContext) : IStorageItemRepository
{
    public async Task<StorageItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using IDocumentSession session = store.LightweightSession();
        return await session.Events.AggregateStreamAsync<StorageItem>(id, token: cancellationToken);
    }
    public async Task StartStreamAsync(StorageItemCreated e, CancellationToken cancellationToken = default)
    {
        await using IDocumentSession session = store.LightweightSession();
        Guid userId = userContext.UserId;
        session.Events.StartStream<StorageItem>(e.Id, e);
        session.LastModifiedBy = userId.ToString();
        await session.SaveChangesAsync(cancellationToken);
    }
    public async Task AppendEventAsync<StorageItemEvent>(Guid id, StorageItemEvent e, CancellationToken cancellationToken = default)
    {
        await using IDocumentSession session = store.LightweightSession();
        Guid userId = userContext.UserId;
        session.Events.Append(id, new { e });
        session.LastModifiedBy = userId.ToString();
        await session.SaveChangesAsync(cancellationToken);
    }
}
