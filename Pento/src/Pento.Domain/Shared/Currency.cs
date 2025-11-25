namespace Pento.Domain.Shared;

public sealed record Currency
{
    internal static readonly Currency None = new("");
    public static readonly Currency Vnd = new("VND");

    private Currency(string code) => Code = code;

    public string Code { get; init; }

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase)) ??
               throw new ApplicationException("The currency code is invalid");
    }

    public static readonly IReadOnlyCollection<Currency> All = new[]
    {
        Vnd
    };
}
