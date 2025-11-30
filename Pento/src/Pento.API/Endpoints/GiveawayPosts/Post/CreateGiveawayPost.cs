using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GiveawayPosts.Create;
using Pento.Domain.GiveawayPosts;

namespace Pento.API.Endpoints.Giveaways.Post;

internal sealed class CreateGiveawayPost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("giveawayposts", async (
            Request dto,
            ICommandHandler<CreateGiveawayPostCommand, Guid> handler,
            CancellationToken ct
        ) =>
        {
            var cmd = new CreateGiveawayPostCommand(
                dto.FoodItemId,
                dto.TitleDescription,
                dto.ContactInfo,
                dto.PickupStartDate,
                dto.PickupEndDate,
                dto.PickupOption,
                dto.Address,
                dto.Quantity
            );

            Domain.Abstractions.Result<Guid> result = await handler.Handle(cmd, ct);

            return result.Match(
                id => Results.Ok(new { GiveawayPostId = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GiveawayPosts)
        .RequireAuthorization();
    }

    internal sealed class Request
    {
        public Guid FoodItemId { get; init; }
        public string TitleDescription { get; init; } = string.Empty;
        public string ContactInfo { get; init; } = string.Empty;
        public DateTime? PickupStartDate { get; init; }
        public DateTime? PickupEndDate { get; init; }
        public PickupOption PickupOption { get; init; }
        public string Address { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
    }
}
