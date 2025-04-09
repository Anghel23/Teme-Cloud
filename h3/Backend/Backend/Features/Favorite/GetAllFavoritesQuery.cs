using Backend.Common;
using MediatR;

namespace Backend.Features.Favorite
{
    public record GetAllFavoritesQuery(Guid UserId) : IRequest<Result<List<FavoriteResponse>>>;
}
