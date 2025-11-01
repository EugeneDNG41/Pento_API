using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.GetCurrent;

public sealed record GetCurrentHouseholdQuery() : IQuery<HouseholdDetailResponse>;

