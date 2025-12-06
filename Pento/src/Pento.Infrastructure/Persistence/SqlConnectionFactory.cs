using System.Data;
using System.Data.Common;
using Npgsql;
using Pento.Application.Abstractions.Data;

namespace Pento.Infrastructure.Persistence;

internal sealed class SqlConnectionFactory(NpgsqlDataSource dataSource) : ISqlConnectionFactory
{
    public async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        return await dataSource.OpenConnectionAsync(cancellationToken);
    }
}
