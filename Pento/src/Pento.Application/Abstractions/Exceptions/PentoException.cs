using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Exceptions;

public sealed class PentoException : Exception
{
    public string RequestName { get; }
    public Error? Error { get; }
    public PentoException(string requestName, Error? error = default, Exception? innerException = default)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }
}
