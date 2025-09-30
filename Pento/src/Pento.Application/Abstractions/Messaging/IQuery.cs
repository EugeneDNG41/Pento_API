using MediatR;
using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
