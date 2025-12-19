using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades.Reports;
public static class TradeReportErrors
{
    public static readonly Error TradeSessionNotFound =
        Error.NotFound("TradeReport.TradeSessionNotFound", "Trade session was not found.");

    public static readonly Error TradeNotCompletedOrCancel =
        Error.Conflict("TradeReport.TradeNotCompletedOrCancel", "Trade session must be completed or cancelled.");

    public static readonly Error CannotReportYourself =
        Error.Conflict("TradeReport.CannotReportYourself", "User cannot report themselves.");

    public static readonly Error InvalidParticipants =
        Error.Forbidden("TradeReport.InvalidParticipants", "User is not a participant of this trade.");

    public static readonly Error DuplicateReport =
        Error.Conflict("TradeReport.DuplicateReport", "You have already reported this trade.");
    public static readonly Error TradeOfferNotFound = 
        Error.NotFound("TradeReport.TradeOfferNotFound", "Trade offer was not found.");
    public static readonly Error TradeRequestNotFound = 
        Error.NotFound("TradeReport.TradeRequestNotFound", "Trade request was not found");
    public static readonly Error NotFound =
        Error.NotFound("TradeReport.ReportNotFound", "Trade report was not found.");
    public static readonly Error InvalidStatus =
        Error.Conflict("TradeReport.InvalidStatus", "The provided status is invalid.");
}
