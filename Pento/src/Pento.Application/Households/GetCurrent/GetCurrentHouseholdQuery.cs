using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Households.GetCurrent;

public sealed record GetCurrentHouseholdQuery;
public sealed record HouseholdDetailResponse(Guid Id, string Name, string? InviteCode, DateTime? InviteCodeExpirationUtc);
public sealed record HouseholdMemberResponse(Guid UserId, string Email, string FirstName, string LastName, Uri? AvatarUrl, IReadOnlyList<string> Roles);
