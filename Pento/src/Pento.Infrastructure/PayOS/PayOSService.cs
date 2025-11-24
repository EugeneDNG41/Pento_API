using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayOS;
using PayOS.Models.V2.PaymentRequests;

namespace Pento.Infrastructure.PayOS;

internal sealed class PayOSService(PayOSOptions options)
{
    public async Task<string> CreatePaymentAsync()
    {
        using var client = new PayOSClient(options);

        return "sad";

    }
}
