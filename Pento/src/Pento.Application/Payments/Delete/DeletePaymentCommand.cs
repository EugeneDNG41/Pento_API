using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Payments.Delete;

public sealed record DeletePaymentCommand(Guid PaymentId) : ICommand;
