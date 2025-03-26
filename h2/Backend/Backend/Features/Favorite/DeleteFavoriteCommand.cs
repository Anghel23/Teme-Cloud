using Backend.Common;
using MediatR;

namespace Backend.Features.Favorite
{
    public record DeleteFavoriteCommand(Guid UserId, string RawgId) : IRequest<Result<Unit>>;
}
