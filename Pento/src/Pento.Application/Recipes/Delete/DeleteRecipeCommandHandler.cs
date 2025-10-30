using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Delete;
internal sealed class DeleteRecipeCommandHandler : ICommandHandler<DeleteRecipeCommand>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        Recipe? recipe = await _recipeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (recipe is null)
        {
            return Result.Failure(RecipeErrors.NotFound);
        }

        _recipeRepository.Remove(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
