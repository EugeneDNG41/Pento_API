using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.Shared;
public sealed record Content
{
    private Content() { }
    public string Value { get; init; } = string.Empty;

    public const int MaxLength = 2000;

    public static Content Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ApplicationException("Content cannot be empty.");
        }

        if (value.Length > MaxLength)
        {
            throw new ApplicationException($"Content cannot exceed {MaxLength} characters.");
        }

        return new Content
        {
            Value = value.Trim()
        };
    }

    public override string ToString() => Value;
}
