using System.Reflection;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Infrastructure.Persistence;

namespace Pento.ArchitectureTests.Infrastructure;

internal abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;

    protected static readonly Assembly DomainAssembly = typeof(BaseEntity).Assembly;

    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;

    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
