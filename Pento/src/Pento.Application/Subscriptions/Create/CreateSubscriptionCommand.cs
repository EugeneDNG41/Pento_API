using System.Data.Common;
using Dapper;
using FluentValidation;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.Create;

public sealed record CreateSubscriptionCommand(string Name, string Description) : ICommand<Guid>;
internal sealed class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Subscription name is required.")
            .MaximumLength(20).WithMessage("Subscription name must not exceed 20 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Subscription description must not exceed 500 characters.");
    }
}

internal sealed class CreateSubscriptionCommandHandler(
    IGenericRepository<Subscription> subscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateSubscriptionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        bool nameTaken = await subscriptionRepository
            .AnyAsync(s => s.Name == command.Name, cancellationToken);
        if (nameTaken)
        {
            return Result.Failure<Guid>(SubscriptionErrors.NameTaken);
        }
        var subscription = Subscription.Create(command.Name, command.Description);
        subscriptionRepository.Add(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return subscription.Id;
    }
}

public sealed record UpdateSubscriptionCommand(Guid Id, string? Name, string? Description) : ICommand;
internal sealed class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subscription Id is required.");
        RuleFor(x => x.Name)
            .MaximumLength(20).WithMessage("Subscription name must not exceed 20 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Subscription description must not exceed 500 characters.");
    }
}
internal sealed class UpdateSubscriptionCommandHandler(
    IGenericRepository<Subscription> subscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSubscriptionCommand>
{
    public async Task<Result> Handle(UpdateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (subscription is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionNotFound);
        }
        if (command.Name != null && subscription.Name != command.Name)
        {
            bool nameTaken = await subscriptionRepository
                .AnyAsync(s => s.Name == command.Name && s.Id != command.Id, cancellationToken);
            if (nameTaken)
            {
                return Result.Failure(SubscriptionErrors.NameTaken);
            }
        }
        subscription.UpdateDetails(command.Name, command.Description);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed record AddSubscriptionPlanCommand(Guid SubscriptionId,
    long PriceAmount,
    string PriceCurrency,
    int? DurationValue,
    TimeUnit? DurationUnit) : ICommand<Guid>;
internal sealed class AddSubscriptionPlanCommandValidator : AbstractValidator<AddSubscriptionPlanCommand>
{
    public AddSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("Subscription Id is required.");
        RuleFor(x => x.PriceAmount)
            .GreaterThan(0).WithMessage("Price amount must be greater than zero.");
        RuleFor(x => x.PriceCurrency)
            .NotEmpty().WithMessage("Price currency is required.")
            .Length(3).WithMessage("Price currency must be a valid 3-letter ISO currency code.");
        RuleFor(x => x.DurationValue)
            .GreaterThan(0).When(x => x.DurationUnit != null)
            .WithMessage("Duration value must be greater than zero when duration unit is specified.");
        RuleFor(x => x.DurationUnit)
            .NotNull().When(x => x.DurationValue != null)
            .WithMessage("Duration unit is required when duration value is specified.");

    }
}

internal sealed class AddSubscriptionPlanCommandHandler(
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddSubscriptionPlanCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddSubscriptionPlanCommand command, CancellationToken cancellationToken)
    {
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
        {
            return Result.Failure<Guid>(SubscriptionErrors.SubscriptionNotFound);
        }
        Duration? duration = command.DurationUnit.HasValue && command.DurationValue.HasValue
            ? new Duration(command.DurationValue.Value, command.DurationUnit.Value)
            : null;
        Result<Currency> currencyResult = Currency.FromCode(command.PriceCurrency);
        if (currencyResult.IsFailure)
        {
            return Result.Failure<Guid>(currencyResult.Error);
        }
        var price = new Money(command.PriceAmount, currencyResult.Value);
        bool duplicatePlan = await subscriptionPlanRepository
            .AnyAsync(sp => sp.SubscriptionId == command.SubscriptionId&& sp.Price == price&& sp.Duration == duration,
                cancellationToken);
        if (duplicatePlan)
        {
            return Result.Failure<Guid>(SubscriptionErrors.DuplicateSubscriptionPlan);
        }
        var plan = SubscriptionPlan.Create(command.SubscriptionId, price, duration);
        subscriptionPlanRepository.Add(plan);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return subscription.Id; //return subscription id
    }
}

public sealed record UpdateSubscriptionPlanCommand(
    Guid Id,
    long? PriceAmount,
    string? PriceCurrency,
    int? DurationValue,
    TimeUnit? DurationUnit) : ICommand;

internal sealed class UpdateSubscriptionPlanCommandValidator : AbstractValidator<UpdateSubscriptionPlanCommand>
{
    public UpdateSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subscription Plan Id is required.");
        RuleFor(x => x.PriceAmount)
            .GreaterThan(0).When(x => x.PriceAmount.HasValue)
            .WithMessage("Price amount must be greater than zero.");
        RuleFor(x => x.PriceCurrency)
            .Length(3).When(x => x.PriceCurrency != null)
            .WithMessage("Price currency must be a valid 3-letter ISO currency code.");
        RuleFor(x => x.DurationValue)
            .GreaterThan(0).When(x => x.DurationUnit != null && x.DurationValue.HasValue)
            .WithMessage("Duration value must be greater than zero when duration unit is specified.");
        RuleFor(x => x.DurationUnit)
            .NotNull().When(x => x.DurationValue != null)
            .WithMessage("Duration unit is required when duration value is specified.");
    }
}

internal sealed class UpdateSubscriptionPlanCommandHandler(
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSubscriptionPlanCommand>
{
    public async Task<Result> Handle(UpdateSubscriptionPlanCommand command, CancellationToken cancellationToken)
    {
        SubscriptionPlan? plan = await subscriptionPlanRepository.GetByIdAsync(command.Id, cancellationToken);
        if (plan is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionPlanNotFound);
        }
        Money? price = null;
        if (command.PriceAmount.HasValue || command.PriceCurrency != null)
        {
            long amount = command.PriceAmount ?? plan.Price.Amount;
            string currencyCode = command.PriceCurrency ?? plan.Price.Currency.Code;
            Result<Currency> currencyResult = Currency.FromCode(currencyCode);
            if (currencyResult.IsFailure)
            {
                return Result.Failure(currencyResult.Error);
            }
            price = new Money(amount, currencyResult.Value);
        }
        Duration? duration = null;
        if (command.DurationUnit.HasValue && command.DurationValue.HasValue)
        {
            duration = new Duration(command.DurationValue.Value, command.DurationUnit.Value);
        }
        bool duplicatePlan = await subscriptionPlanRepository
            .AnyAsync(sp => sp.Id != command.Id && sp.Price == price && sp.Duration == duration,
                cancellationToken);
        if (duplicatePlan)
        {
            return Result.Failure(SubscriptionErrors.DuplicateSubscriptionPlan);
        }
        plan.UpdateDetails(price, duration);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed record AddSubscriptionFeatureCommand(
    Guid SubscriptionId,
    string FeatureName,
    int? EntitlementQuota,
    TimeUnit? EntitlementResetPer) : ICommand<Guid>;

internal sealed class AddSubscriptionFeatureCommandValidator : AbstractValidator<AddSubscriptionFeatureCommand>
{
    public AddSubscriptionFeatureCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("Subscription Id is required.");
        RuleFor(x => x.FeatureName)
            .NotEmpty().WithMessage("Feature name is required.")
            .MaximumLength(50).WithMessage("Feature name must not exceed 100 characters.");
        RuleFor(x => x.EntitlementQuota)
            .GreaterThan(0).When(x => x.EntitlementResetPer != null)
            .WithMessage("Entitlement quota must be greater than zero when entitlement reset period is specified.");
    }
}

internal sealed class AddSubscriptionFeatureCommandHandler(
    IGenericRepository<Feature> featureRepository,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddSubscriptionFeatureCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddSubscriptionFeatureCommand command, CancellationToken cancellationToken)
    {
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
        {
            return Result.Failure<Guid>(SubscriptionErrors.SubscriptionNotFound);
        }
        Feature? feature = (await featureRepository
            .FindAsync(f => f.Name == command.FeatureName, cancellationToken)).SingleOrDefault();
        if (feature is null)
        {
            return Result.Failure<Guid>(FeatureErrors.NotFound);
        }
        
        bool duplicateFeature = await subscriptionFeatureRepository
            .AnyAsync(sf => sf.SubscriptionId == command.SubscriptionId && sf.FeatureName == command.FeatureName,
                cancellationToken);
        if (duplicateFeature)
        {
            return Result.Failure<Guid>(SubscriptionErrors.DuplicateSubscriptionFeature);
        }
        Limit? entitlement = command.EntitlementQuota.HasValue
            ? new Limit(command.EntitlementQuota.Value, command.EntitlementResetPer)
            : null;
        var subscriptionFeature = SubscriptionFeature.Create(
            command.SubscriptionId,
            feature.Name,
            entitlement);
        subscriptionFeatureRepository.Add(subscriptionFeature);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return subscription.Id;
    }
}

public sealed record UpdateSubscriptionFeatureCommand(
    Guid Id,
    string? FeatureName,
    int? EntitlementQuota,
    TimeUnit? EntitlementResetPer) : ICommand;

internal sealed class UpdateSubscriptionFeatureCommandValidator : AbstractValidator<UpdateSubscriptionFeatureCommand>
{
    public UpdateSubscriptionFeatureCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subscription Feature Id is required.");
        RuleFor(x => x.FeatureName)
            .MaximumLength(50).WithMessage("Feature name must not exceed 100 characters.");
        RuleFor(x => x.EntitlementQuota)
            .GreaterThan(0).When(x => x.EntitlementResetPer != null)
            .WithMessage("Entitlement quota must be greater than zero when entitlement reset period is specified.");
    }
}

internal sealed class UpdateSubscriptionFeatureCommandHandler(
    IGenericRepository<Feature> featureRepository,
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSubscriptionFeatureCommand>
{
    public async Task<Result> Handle(UpdateSubscriptionFeatureCommand command, CancellationToken cancellationToken)
    {
        SubscriptionFeature? subscriptionFeature = await subscriptionFeatureRepository.GetByIdAsync(command.Id, cancellationToken);
        if (subscriptionFeature is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionFeatureNotFound);
        }
        Feature? feature = (await featureRepository
            .FindAsync(f => f.Name == command.FeatureName, cancellationToken)).SingleOrDefault();
        if (feature is null)
        {
            return Result.Failure(FeatureErrors.NotFound);
        }
        bool duplicateFeature = await subscriptionFeatureRepository
            .AnyAsync(sf => sf.Id != command.Id && sf.FeatureName == command.FeatureName,
                cancellationToken);
        if (duplicateFeature)
        {
            return Result.Failure(SubscriptionErrors.DuplicateSubscriptionFeature);
        }
        Limit? entitlement = command.EntitlementQuota.HasValue
            ? new Limit(command.EntitlementQuota.Value, command.EntitlementResetPer)
            : null;
        subscriptionFeature.UpdateDetails(feature.Name, entitlement);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed record GetFeaturesQuery : IQuery<IReadOnlyList<FeatureResponse>>;

internal sealed class GetFeaturesQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetFeaturesQuery, IReadOnlyList<FeatureResponse>>
{
    public async Task<Result<IReadOnlyList<FeatureResponse>>> Handle(GetFeaturesQuery request, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                name AS Name
            FROM Features
            ORDER BY name;
        ";
        CommandDefinition command = new(sql, cancellationToken: cancellationToken);
        IEnumerable<FeatureResponse> features = await connection.QueryAsync<FeatureResponse>(command);
        return features.ToList();
    }
}

public sealed record FeatureResponse(string Name);
