using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;


namespace Pento.Application.FoodReferences.Import;
public sealed record ImportFoodReferencesCommand(IFormFile File) : ICommand<int>;

