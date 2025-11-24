using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Payments.Cancel;

public sealed record CancelPaymentCommand(Guid PaymentId, string? Reason) : ICommand;
