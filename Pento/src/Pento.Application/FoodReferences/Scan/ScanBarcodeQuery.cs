using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Get;

namespace Pento.Application.FoodReferences.Scan;

public sealed record ScanBarcodeQuery(string Barcode) : IQuery<FoodReferenceResponse>;
