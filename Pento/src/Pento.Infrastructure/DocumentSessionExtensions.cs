using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten;

namespace Pento.Infrastructure;

public static class DocumentSessionExtensions
{
    public static async Task Add<T>(this IDocumentSession session, 
        Guid id, object @event, CancellationToken cancellationToken)
        where T : class
    {
        session.Events.StartStream<T>(id, @event);
        await session.SaveChangesAsync(cancellationToken);
    }

    public static  Task GetAndUpdate<T>(this IDocumentSession session,
        Guid id, int version,
        Func<T, object> handle, CancellationToken cancellationToken)
        where T : class => session.Events.WriteToAggregate<T>(id, version, stream => 
            stream.AppendOne(handle(stream.Aggregate!)), cancellationToken);
}
