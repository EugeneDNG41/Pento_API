using System.Data;
using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.Units;

namespace Pento.Application.FoodItemLogs.GetSummary;

internal sealed class GetAdminFoodItemLogSummaryQueryHandler(
    IGenericRepository<Unit> unitRepository,
    ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetAdminFoodItemLogSummaryQuery, FoodSummary>
{
    public async Task<Result<FoodSummary>> Handle(GetAdminFoodItemLogSummaryQuery query, CancellationToken cancellationToken)
    {

        Unit? weightUnit = query.WeightUnitId.HasValue
            ? await unitRepository.GetByIdAsync(query.WeightUnitId.Value, cancellationToken)
            : (await unitRepository.FindAsync(u => u.Type == UnitType.Weight && u.ToBaseFactor == 1, cancellationToken)).FirstOrDefault();
        Unit? volumeUnit = query.VolumeUnitId.HasValue
            ? await unitRepository.GetByIdAsync(query.VolumeUnitId.Value, cancellationToken)
            : (await unitRepository.FindAsync(u => u.Type == UnitType.Volume && u.ToBaseFactor == 1, cancellationToken)).FirstOrDefault();
        if (weightUnit is null || volumeUnit is null)
        {
            return Result.Failure<FoodSummary>(UnitErrors.NotFound);
        }
        if (weightUnit.Type != UnitType.Weight || volumeUnit.Type != UnitType.Volume)
        {
            return Result.Failure<FoodSummary>(UnitErrors.InvalidConversion);
        }
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("HouseholdId", query.HouseholdId, DbType.Guid);
        parameters.Add("FromUtc", query.FromUtc);
        parameters.Add("ToUtc", query.ToUtc);
        parameters.Add("IsDeleted", query.IsDeleted, DbType.Boolean);

        parameters.Add("WeightType", UnitType.Weight.ToString(), DbType.String);
        parameters.Add("VolumeType", UnitType.Volume.ToString(), DbType.String);

        parameters.Add("IntakeAction", FoodItemLogAction.Intake.ToString(), DbType.String);
        parameters.Add("ConsumptionAction", FoodItemLogAction.Consumption.ToString(), DbType.String);
        parameters.Add("DiscardAction", FoodItemLogAction.Discard.ToString(), DbType.String);

        parameters.Add("WeightToBaseFactor", weightUnit.ToBaseFactor, DbType.Decimal);
        parameters.Add("VolumeToBaseFactor", volumeUnit.ToBaseFactor, DbType.Decimal);

        const string sql = $"""
        SELECT
            COALESCE(SUM(
                CASE 
                    WHEN u.type = @WeightType 
                         AND l.action = @IntakeAction 
                    THEN l.quantity * @WeightToBaseFactor / u.to_base_factor
                END
            ), 0) AS IntakeByWeight,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @VolumeType 
                         AND l.action = @IntakeAction 
                    THEN l.quantity * @VolumeToBaseFactor / u.to_base_factor
                END
            ), 0) AS IntakeByVolume,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @WeightType 
                         AND l.action = @ConsumptionAction 
                    THEN l.quantity * @WeightToBaseFactor / u.to_base_factor
                END
            ), 0) AS ConsumptionByWeight,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @VolumeType 
                         AND l.action = @ConsumptionAction 
                    THEN l.quantity * @VolumeToBaseFactor / u.to_base_factor
                END
            ), 0) AS ConsumptionByVolume,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @WeightType 
                         AND l.action = @DiscardAction 
                    THEN l.quantity * @WeightToBaseFactor / u.to_base_factor
                END
            ), 0) AS DiscardByWeight,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @VolumeType 
                         AND l.action = @DiscardAction 
                    THEN l.quantity * @VolumeToBaseFactor  / u.to_base_factor
                END
            ), 0) AS DiscardByVolume
        FROM food_item_logs l
        JOIN units u ON u.id = l.unit_id
        WHERE (@IsDeleted is null OR l.is_deleted = @IsDeleted )
            AND (@HouseholdId is null OR l.household_id = @HouseholdId)
            AND l.timestamp >= COALESCE(@FromUtc, l.timestamp)
            AND l.timestamp <= COALESCE(@ToUtc, l.timestamp);

        SELECT
            COUNT (*) AS TotalFoodItems,
            COUNT(*) FILTER (WHERE (f.expiration_date::date - current_date) > 3) AS FreshCount,
            COUNT(*) FILTER (WHERE (f.expiration_date::date - current_date) BETWEEN 0 AND 3) AS ExpiringCount,
            COUNT(*) FILTER (WHERE (f.expiration_date::date - current_date) < 0) AS ExpiredCount,
        	COALESCE(SUM(
                CASE 
                    WHEN u.type = @WeightType
                        AND (f.expiration_date::date - current_date) > 3
                    THEN f.quantity * @WeightToBaseFactor / u.to_base_factor
                END
            ), 0) AS FreshByWeight,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @VolumeType 
                        AND (f.expiration_date::date - current_date) > 3
                    THEN f.quantity * @VolumeToBaseFactor  / u.to_base_factor
                END
            ), 0) AS FreshByVolume,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @WeightType 
                         AND (f.expiration_date::date - current_date) >= 0
        				 AND (f.expiration_date::date - current_date) <= 3
                    THEN f.quantity * @WeightToBaseFactor / u.to_base_factor
                END
            ), 0) AS ExpiringByWeight,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @VolumeType  
                         AND (f.expiration_date::date - current_date) >= 0
        				 AND (f.expiration_date::date - current_date) <= 3 
                    THEN f.quantity * @VolumeToBaseFactor  / u.to_base_factor
                END
            ), 0) AS ExpiringByVolume,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @WeightType
                         AND (f.expiration_date::date - current_date) < 0 
                    THEN f.quantity * @WeightToBaseFactor / u.to_base_factor
                END
            ), 0) AS ExpiredByWeight,

            COALESCE(SUM(
                CASE 
                    WHEN u.type = @VolumeType 
                         AND (f.expiration_date::date - current_date) < 0
                    THEN f.quantity * @VolumeToBaseFactor  / u.to_base_factor
                END
            ), 0) AS ExpiredByVolume

        FROM food_items f
        JOIN units u ON u.id = f.unit_id
        JOIN food_item_logs fil ON fil.food_item_id = f.id
        WHERE (@IsDeleted is null OR f.is_deleted = @IsDeleted )
            AND (@HouseholdId is null OR f.household_id = @HouseholdId)
            AND fil.timestamp >= COALESCE(@FromUtc, fil.timestamp)
            AND fil.timestamp <= COALESCE(@ToUtc, fil.timestamp);

        
        """;

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        FoodItemLogSummary logSummary = await multi.ReadFirstAsync<FoodItemLogSummary>();
        FoodItemSummary itemSummary = await multi.ReadFirstAsync<FoodItemSummary>();
        var foodSummary = new FoodSummary(
            WeightUnit: weightUnit.Name,
            VolumeUnit: volumeUnit.Name,
            LogSummary: logSummary,
            FoodItemSummary: itemSummary
            );
        return foodSummary;
    }
}
