using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Abstractions.External.PayOS;

public sealed record PaymentLinkResponse(Guid PaymentId, Uri CheckoutUrl, string QrCode);

