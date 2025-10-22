using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Pento.Infrastructure.Data;
public class UriTypeHandler : SqlMapper.TypeHandler<Uri?>
{
    public override void SetValue(IDbDataParameter parameter, Uri? value)
    {
        parameter.Value = value?.ToString() ?? (object)DBNull.Value;
    }

    public override Uri? Parse(object value)
    {
        if (value is string s && !string.IsNullOrWhiteSpace(s))
        {
            return Uri.TryCreate(s, UriKind.Absolute, out Uri? uri) ? uri : null;
        }
        return null;
    }
}

