using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Update;

internal sealed class UpdateRecipeCommandHandler : ICommandHandler<UpdateRecipeCommand>
{
    private readonly IGenericRepository<Recipe> _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateRecipeCommandHandler(
        IGenericRepository<Recipe> recipeRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _recipeRepository = recipeRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        Recipe? recipe = await _recipeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (recipe is null)
        {
            return Result.Failure(RecipeErrors.NotFound);
        }

        _ = TimeRequirement.Create(request.PrepTimeMinutes, request.CookTimeMinutes);

        recipe.UpdateDetails(
            request.Title,
            request.Description,
            request.Notes,
            request.Servings,
            request.DifficultyLevel,
            request.ImageUrl,
            _dateTimeProvider.UtcNow
        );

        recipe.ChangeVisibility(request.IsPublic, _dateTimeProvider.UtcNow);
        await _recipeRepository.UpdateAsync(recipe, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
