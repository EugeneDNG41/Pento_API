using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Delete;
internal sealed class DeleteRecipeCommandHandler : ICommandHandler<DeleteRecipeCommand>
{
    private readonly IGenericRepository<Recipe> _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRecipeCommandHandler(
        IGenericRepository<Recipe> recipeRepository,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        Recipe? recipe = await _recipeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (recipe is null || recipe.IsDeleted)
        {
            return Result.Failure(RecipeErrors.NotFound);
        }
        recipe.Delete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
