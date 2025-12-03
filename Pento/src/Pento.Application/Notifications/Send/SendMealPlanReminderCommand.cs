using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Notifications.Send;
public sealed record SendMealPlanReminderCommand(Guid MealPlanId)
    : ICommand<Guid>;

