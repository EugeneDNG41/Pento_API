using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Get;

namespace Pento.Application.FoodReferences.Scan;

public sealed record ScanBarcodeQuery(string Barcode) : IQuery<FoodReferenceResponse>;
