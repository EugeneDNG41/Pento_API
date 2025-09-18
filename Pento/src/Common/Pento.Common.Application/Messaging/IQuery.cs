using MediatR;
using Pento.Common.Domain;

namespace Pento.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
