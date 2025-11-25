using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Payments.GetById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IQuery<PaymentResponse>;
