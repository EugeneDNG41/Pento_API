using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Authentication;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Users;
using Pento.Infrastructure;
using Pento.Infrastructure.Repositories;

namespace Pento.Infrastructure.Repositories;

internal sealed class FoodItemRepository(IDocumentStore store, IUserContext userContext) : IFoodItemRepository
{
    public async Task<FoodItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using IDocumentSession session = store.LightweightSession();
        return await session.Events.AggregateStreamAsync<FoodItem>(id, token: cancellationToken);
    }
    public async Task StartStreamAsync(FoodItemAdded e, CancellationToken cancellationToken = default)
    {
        await using IDocumentSession session = store.LightweightSession();
        Guid userId = userContext.UserId;
        session.Events.StartStream<FoodItem>(e.Id, e);
        session.LastModifiedBy = userId.ToString();
        await session.SaveChangesAsync(cancellationToken);
    }
    public async Task AppendEventAsync<FoodItemEvent>(Guid id, FoodItemEvent e, CancellationToken cancellationToken = default)
    {
        await using IDocumentSession session = store.LightweightSession();
        Guid userId = userContext.UserId;
        session.Events.Append(id, new { e });
        session.LastModifiedBy = userId.ToString();
        await session.SaveChangesAsync(cancellationToken);
    }
}
