using Microsoft.AspNetCore.Mvc;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Households;
using Pento.Domain.Milestones;
using Pento.Domain.Units;
using Pento.Domain.UserActivities;
using Pento.Domain.UserMilestones;
using Pento.Domain.Users;

namespace Pento.Application.EventHandlers;

internal sealed class FoodItemAddedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Unit> unitRepository,
    IGenericRepository<FoodItemLog> logRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<FoodItemAddedDomainEvent>
{
    public override async Task Handle(
        FoodItemAddedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(domainEvent.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            throw new PentoException(nameof(FoodItemAddedEventHandler), FoodItemErrors.NotFound);
        }
        Unit? unit = await unitRepository.GetByIdAsync(domainEvent.UnitId, cancellationToken);
        if (unit is null)
        {
            throw new PentoException(nameof(FoodItemAddedEventHandler), FoodItemErrors.InvalidMeasurementUnit);
        }
        var log = FoodItemLog.Create(
            foodItem.Id,
            foodItem.HouseholdId,
            domainEvent.UserId,
            domainEvent.Timestamp,
            FoodItemLogAction.Intake,
            domainEvent.Quantity,
            unit.Id);
        logRepository.Add(log);
        Result<UserActivity> intakeResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            foodItem.HouseholdId,
            ActivityCode.FOOD_ITEM_INTAKE.ToString(),
            foodItem.Id,
            cancellationToken);
        if (intakeResult.IsFailure)
        {
            throw new PentoException(nameof(FoodItemAddedEventHandler), intakeResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(intakeResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class MilestoneEnabledEventHandler(
    IMilestoneService milestoneService,
    IGenericRepository<Milestone> milestoneRepository,
    IGenericRepository<UserMilestone> userMilestoneRepository,
    IGenericRepository<User> userRepository,

    IUnitOfWork unitOfWork) : DomainEventHandler<MilestoneEnabledDomainEvent>
{
    public async override Task Handle(MilestoneEnabledDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Milestone? milestone = await milestoneRepository.GetByIdAsync(domainEvent.MilestoneId, cancellationToken);
        if (milestone == null)
        {
            throw new PentoException(nameof(MilestoneEnabledEventHandler), MilestoneErrors.NotFound);
        }
        var alreadyEarnedUserIds = (await userMilestoneRepository.FindAsync(
            um => um.MilestoneId == milestone.Id,
            cancellationToken)).Select(um => um.UserId).ToHashSet();
        var notEarnedUserIds = (await userRepository.FindAsync(
            u => !alreadyEarnedUserIds.Contains(u.Id),
            cancellationToken)).Select(u => u.Id).ToHashSet();
        foreach (Guid userId in notEarnedUserIds)
        {
            Result checkResult = await milestoneService.CheckUserMilestoneAsync(userId, milestone, cancellationToken);
            if (checkResult.IsFailure)
            {
                throw new PentoException(nameof(MilestoneEnabledEventHandler), checkResult.Error);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
