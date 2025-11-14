using System.Data;
using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.Households;
using Pento.Domain.Units;

namespace Pento.Application.FoodItemLogs.GetSummary;

internal sealed class GetFoodItemLogSummaryQueryHandler(
    IUserContext userContext,
    IGenericRepository<Unit> unitRepository,
    ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetFoodItemLogSummaryQuery, FoodItemLogSummary>
{
    public async Task<Result<FoodItemLogSummary>> Handle(GetFoodItemLogSummaryQuery query, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<FoodItemLogSummary>(HouseholdErrors.NotInAnyHouseHold);
        }
        Unit? weightUnit = query.WeightUnitId.HasValue
            ? await unitRepository.GetByIdAsync(query.WeightUnitId.Value, cancellationToken)
            : (await unitRepository.FindAsync(u => u.Type == UnitType.Weight && u.ToBaseFactor == 1, cancellationToken)).FirstOrDefault();
        Unit? volumeUnit = query.VolumeUnitId.HasValue
            ? await unitRepository.GetByIdAsync(query.VolumeUnitId.Value, cancellationToken)
            : (await unitRepository.FindAsync(u => u.Type == UnitType.Volume && u.ToBaseFactor == 1, cancellationToken)).FirstOrDefault();
        if (weightUnit is null || volumeUnit is null)
        {
            return Result.Failure<FoodItemLogSummary>(UnitErrors.NotFound);
        }
        if (weightUnit.Type != UnitType.Weight || volumeUnit.Type != UnitType.Volume)
        {
            return Result.Failure<FoodItemLogSummary>(UnitErrors.InvalidConversion);
        }
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();
        var filters = new List<string>
        {
            "l.is_deleted IS FALSE",
            "l.is_archived IS FALSE",
            "l.household_id = @HouseholdId"
        };
        var parameters = new DynamicParameters();
        parameters.Add("HouseholdId", householdId.Value, DbType.Guid);
        if (query.FromUtc is not null)
        {
            filters.Add("fil.timestamp >= @FromUtc");
            parameters.Add("FromUtc", query.FromUtc);
        }
        if (query.ToUtc is not null)
        {
            filters.Add("fil.timestamp <= @ToUtc");
            parameters.Add("ToUtc", query.ToUtc);
        }
        parameters.Add("WeightType", UnitType.Weight.ToString(), DbType.String);
        parameters.Add("VolumeType", UnitType.Volume.ToString(), DbType.String);

        parameters.Add("IntakeAction", FoodItemLogAction.Intake.ToString(), DbType.String);
        parameters.Add("ConsumptionAction", FoodItemLogAction.Consumption.ToString(), DbType.String);
        parameters.Add("DiscardAction", FoodItemLogAction.Discard.ToString(), DbType.String);

        parameters.Add("WeightToBaseFactor", weightUnit.ToBaseFactor, DbType.Decimal);
        parameters.Add("VolumeToBaseFactor", volumeUnit.ToBaseFactor, DbType.Decimal);

        parameters.Add("WeightUnit", weightUnit.Name ?? string.Empty, DbType.String);
        parameters.Add("VolumeUnit", volumeUnit.Name ?? string.Empty, DbType.String);

        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;

        string sql = $"""
        SELECT
            COALESCE(SUM(
                CASE 
                    WHEN u.type = @WeightType 
                         AND l.action = @IntakeAction 
                    THEN l.quantity * u.to_base_factor / @WeightToBaseFactor
                END
            ), 0) AS TotalIntakeByWeight,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @VolumeType 
                         AND l.action = @IntakeAction 
                    THEN l.quantity * u.to_base_factor / @VolumeToBaseFactor
                END
            ), 0) AS TotalIntakeByVolume,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @WeightType 
                         AND l.action = @ConsumptionAction 
                    THEN l.quantity * u.to_base_factor / @WeightToBaseFactor
                END
            ), 0) AS TotalConsumptionByWeight,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @VolumeType 
                         AND l.action = @ConsumptionAction 
                    THEN l.quantity * u.to_base_factor / @VolumeToBaseFactor
                END
            ), 0) AS TotalConsumptionByVolume,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @WeightType 
                         AND l.action = @DiscardAction 
                    THEN l.quantity * u.to_base_factor / @WeightToBaseFactor
                END
            ), 0) AS TotalDiscardByWeight,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @VolumeType 
                         AND l.action = @DiscardAction 
                    THEN l.quantity * u.to_base_factor / @VolumeToBaseFactor
                END
            ), 0) AS TotalDiscardByVolume,

            CAST(@WeightUnit AS text) AS WeightUnit,
            CAST(@VolumeUnit AS text) AS VolumeUnit
        FROM food_item_logs l
        JOIN units u ON u.id = l.unit_id
        {whereClause};
        """;

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        FoodItemLogSummary summary =
            await connection.QuerySingleAsync<FoodItemLogSummary>(command);
        if (summary == null)
        {
            return Result.Failure<FoodItemLogSummary>(FoodItemLogErrors.NotFound);
        }
        return summary;
    }
}
