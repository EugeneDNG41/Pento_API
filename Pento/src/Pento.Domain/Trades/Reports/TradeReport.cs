using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades.Reports;

public sealed class TradeReport : Entity
{
    private TradeReport() { }

    public Guid TradeSessionId { get; private set; }
    public Guid ReporterUserId { get; private set; }

    public TradeReportReason Reason { get; private set; }
    public FoodSafetyIssueLevel Severity { get; private set; }

    public string? Description { get; private set; }


    public TradeReportStatus Status { get; private set; }

    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    public TradeReport(
        Guid id,
        Guid tradeSessionId,
        Guid reporterUserId,
        TradeReportReason reason,
        FoodSafetyIssueLevel severity,
        string? description,
        DateTime createdOn
    ) : base(id)
    {
        TradeSessionId = tradeSessionId;
        ReporterUserId = reporterUserId;
        Reason = reason;
        Severity = severity;
        Description = description;
        Status = TradeReportStatus.Pending;
        CreatedOn = createdOn;
        UpdatedOn = null;
    }

    public static TradeReport Create(
        Guid tradeSessionId,
        Guid reporterUserId,
        TradeReportReason reason,
        FoodSafetyIssueLevel severity,
        string? description,
        DateTime createdOn
    )
    {
        return new TradeReport(
            id: Guid.CreateVersion7(),
            tradeSessionId: tradeSessionId,
            reporterUserId: reporterUserId,
            reason: reason,
            severity: severity,
            description: description,
            createdOn: createdOn
        );
    }

    public void MarkUnderReview(DateTime updatedOn)
    {
        Status = TradeReportStatus.UnderReview;
        UpdatedOn = updatedOn;
    }

    public void Resolve(DateTime updatedOn)
    {
        Status = TradeReportStatus.Resolved;
        UpdatedOn = updatedOn;
    }

    public void Reject(DateTime updatedOn)
    {
        Status = TradeReportStatus.Rejected;
        UpdatedOn = updatedOn;
    }
}
public enum TradeReportReason
    {
        FoodSafetyConcern,
        ExpiredFood,
        PoorHygiene,
        MisleadingInformation,
        InappropriateBehavior,
        Other
    }

    public enum FoodSafetyIssueLevel
    {
        Minor,
        Serious,
        Critical
    }

    public enum TradeReportStatus
    {
        Pending,
        UnderReview,
        Resolved,
        Rejected
    }


