using login.api.data;
using login.api.dto;
using Microsoft.EntityFrameworkCore;

namespace login.api.services;

public interface IDashboardService
{
    Task<(List<ReadUserDto>? users, string? error)> GetAllUsersAsync();
    Task<(ReadUserDto? user, string? error)> GetUserAsync(int userId);
    Task<(ReadGameDto? game, string? error)> PostGameAsync(CreateGameDto request);
    Task<string?> AddGameToAccountAsync(int userId, string gameTitle);
    Task<(List<ReadGameDto>? games, string? error)> GetAllGamesAsync();
    Task<(ReadGameDto? game, string? error)> GetGameAsync(int gameId);
}

public class ImplementedDashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    private readonly IigdbService _igdbService;
    public ImplementedDashboardService(AppDbContext db, IigdbService igdb)
    {
        _db = db;
        _igdbService = igdb;
    }


    public async Task<(List<ReadUserDto>? users, string? error)> GetAllUsersAsync()
    {
        var users = await _db.AppUsers.Include(u => u.Games).ToListAsync();

        if(users.Count <= 0)
        {
            return (null, "No users were found in database");
        }

        List<ReadUserDto> safeList = new List<ReadUserDto>();

        foreach(var user in users)
        {
            var readUser = new ReadUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = user.Role,
                ProfilePicture = user.ProfilePicture,
                Games = user.Games.Select(g => g.Title).ToList(),
                GamesCoverArt = user.Games.Select(g => g.CoverArt).ToList()
            };

            safeList.Add(readUser);
        }

        return (safeList, null);
    }



    public async Task<(ReadUserDto? user, string? error)> GetUserAsync(int userId)
    {
        var existingUser = await _db.AppUsers.Include(u => u.Games).FirstOrDefaultAsync(u => u.Id == userId);

        if(existingUser == null)
        {
            return (null, "A user with this ID was not found");
        }

        var readUser = new ReadUserDto
        {
            Id = existingUser.Id,
            UserName = existingUser.UserName,
            Role = existingUser.Role,
            ProfilePicture = existingUser.ProfilePicture,
            Games = existingUser.Games.Select(g => g.Title).ToList(),
            GamesCoverArt = existingUser.Games.Select(g => g.CoverArt).ToList()
        };

        return (readUser, null);
    }



    public async Task<(ReadGameDto? game, string? error)> PostGameAsync(CreateGameDto request)
    {
        var existingGame = await _db.Games.FirstOrDefaultAsync(g => g.Title == request.Title);

        if(existingGame != null)
        {
            return (null, "A game with this title is already in database");
        }

        var (coverURL, error) = await _igdbService.GetGameCoverUrlAsync(request.Title);

        if(error != null)
        {
            return (null, error);
        }

        var newGame = new Game
        {
            Title = request.Title,
            CoverArt = coverURL
        };

        _db.Games.Add(newGame);
        await _db.SaveChangesAsync();

        var safeResponse = new ReadGameDto
        {
            Id = newGame.Id,
            Title = newGame.Title,
            CoverArt = newGame.CoverArt
        };

        return (safeResponse, null);
    }




    public async Task<string?> AddGameToAccountAsync(int userId, string gameTitle)
    {
        var existingUser = await _db.AppUsers.Include(u => u.Games).FirstOrDefaultAsync(u => u.Id == userId);
        var existingGame = await _db.Games.FirstOrDefaultAsync(g => g.Title == gameTitle);
        
        if(existingUser == null)
        {
            return "A user with this ID was not found";
        }

        if(existingGame == null)
        {
            return "A game with this title was not found";
        }

        bool isRelated = existingUser.Games.Any(g => g.Title == existingGame.Title);

        if (isRelated)
        {
            return "BAD_REQUEST A game with this title is already in the user's account";
        }

        existingUser.Games.Add(existingGame);
        await _db.SaveChangesAsync();

        return null;
    }




    public async Task<(List<ReadGameDto>? games, string? error)> GetAllGamesAsync()
    {
        var games = await _db.Games.Include(g => g.Users).ToListAsync();

        if(games.Count <= 0)
        {
            return (null, "No games found in database");
        }

        List<ReadGameDto> safeList = new List<ReadGameDto>();

        foreach(var game in games)
        {
            var gameRead = new ReadGameDto
            {
                Id = game.Id,
                Title = game.Title,
                CoverArt = game.CoverArt,
                UserNames = game.Users.Select(u => u.UserName).ToList()
            };

            safeList.Add(gameRead);
        }

        return (safeList, null);
    }




    public async Task<(ReadGameDto? game, string? error)> GetGameAsync(int gameId)
    {
        var existingGame = await _db.Games.Include(g => g.Users).FirstOrDefaultAsync(g => g.Id == gameId);

        if(existingGame == null)
        {
            return (null, "A game with this ID was not found");
        }

        var safeResponse = new ReadGameDto
        {
            Id = existingGame.Id,
            Title = existingGame.Title,
            CoverArt = existingGame.CoverArt,
            UserNames = existingGame.Users.Select(u => u.UserName).ToList()
        };

        return (safeResponse, null);
    }
}  