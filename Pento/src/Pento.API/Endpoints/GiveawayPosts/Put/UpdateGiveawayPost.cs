using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GiveawayPosts.Update;
using Pento.Domain.GiveawayPosts;

namespace Pento.API.Endpoints.GiveawayPosts.Put;

internal sealed class UpdateGiveawayPost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("giveawayposts/{id:guid}", async (
            Guid id,
            Request dto,
            ICommandHandler<UpdateGiveawayPostCommand, Guid> handler,
            CancellationToken ct
        ) =>
        {
            var cmd = new UpdateGiveawayPostCommand(
                id,
                dto.TitleDescription,
                dto.ContactInfo,
                dto.Address,
                dto.Quantity,
                dto.PickupStartDate,
                dto.PickupEndDate,
                dto.PickupOption,
                dto.Status
            );

            Domain.Abstractions.Result<Guid> result = await handler.Handle(cmd, ct);

            return result.Match(
                _ => Results.Ok(id),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GiveawayPosts)
        .RequireAuthorization();
    }

    internal sealed class Request
    {
        public string? TitleDescription { get; init; }
        public string? ContactInfo { get; init; }
        public string? Address { get; init; }
        public decimal? Quantity { get; init; }
        public DateTime? PickupStartDate { get; init; }
        public DateTime? PickupEndDate { get; init; }
        public PickupOption? PickupOption { get; init; }
        public GiveawayStatus Status { get; init; }
    }
}
