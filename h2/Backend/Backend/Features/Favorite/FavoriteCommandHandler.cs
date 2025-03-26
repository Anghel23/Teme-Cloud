using Backend.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Favorite
{
    public class FavoriteCommandHandler :
        IRequestHandler<CreateFavoriteCommand, Result<FavoriteResponse>>,
        IRequestHandler<DeleteFavoriteCommand, Result<Unit>>,
        IRequestHandler<GetAllFavoritesQuery, Result<List<FavoriteResponse>>>
    {
        private readonly AppDbContext dbContext;

        public FavoriteCommandHandler(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Result<FavoriteResponse>> Handle(CreateFavoriteCommand request, CancellationToken cancellationToken)
        {
            var userExists = await dbContext.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
            if (!userExists)
            {
                return Result<FavoriteResponse>.Failure("User not found");
            }

            var favorite = new Entities.Favorite
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                RawgId = request.RawgId
            };

            await dbContext.Favorites.AddAsync(favorite, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var response = new FavoriteResponse(favorite.Id, favorite.RawgId);
            return Result<FavoriteResponse>.Success(response);
        }

        public async Task<Result<Unit>> Handle(DeleteFavoriteCommand request, CancellationToken cancellationToken)
        {
            var favorite = await dbContext.Favorites
                .FirstOrDefaultAsync(f => f.UserId == request.UserId && f.RawgId == request.RawgId, cancellationToken);
            if (favorite == null)
            {
                return Result<Unit>.Failure("Favorite not found");
            }

            dbContext.Favorites.Remove(favorite);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }

        public async Task<Result<List<FavoriteResponse>>> Handle(GetAllFavoritesQuery request, CancellationToken cancellationToken)
        {
            var favorites = await dbContext.Favorites
                .Where(f => f.UserId == request.UserId)
                .Select(f => new FavoriteResponse(f.Id, f.RawgId))
                .ToListAsync(cancellationToken);

            return Result<List<FavoriteResponse>>.Success(favorites);
        }
    }
}
