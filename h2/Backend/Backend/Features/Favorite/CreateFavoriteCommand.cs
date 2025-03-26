using Backend.Common;
using MediatR;

namespace Backend.Features.Favorite
{
    public record CreateFavoriteCommand(string RawgId, Guid UserId) : IRequest<Result<FavoriteResponse>>;
}
