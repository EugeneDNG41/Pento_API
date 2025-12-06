using System.Data.Common;

namespace Pento.Application.Abstractions.Persistence;

public interface ISqlConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken);
}
