using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Pento.API.Extensions;

internal static class ETagExtensions
{
    public static int ToExpectedVersion(string? eTag)
    {
        ArgumentNullException.ThrowIfNull(eTag);
        var parsed = EntityTagHeaderValue.Parse(eTag);
        if (parsed.IsWeak)
        {
            throw new ArgumentException("Weak ETags are not accepted. Use a strong ETag.", nameof(eTag));
        }

        string? value = parsed.Tag.Value;
        if (string.IsNullOrEmpty(value) || value.Length < 2 || value[0] != '"' || value[^1] != '"')
        {
            throw new ArgumentException("ETag must be a quoted integer like \"123\".", nameof(eTag));
        }

        if (!int.TryParse(value.AsSpan(1, value.Length - 2), out int version))
        {
            throw new ArgumentException("ETag inner value must be an integer.", nameof(eTag));
        }

        return version;
    }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
internal sealed class FromIfMatchHeaderAttribute : FromHeaderAttribute
{
    public FromIfMatchHeaderAttribute()
    {
        Name = "If-Match";
    }
}
