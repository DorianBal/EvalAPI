using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Autoriser uniquement les utilisateurs authentifiés
    public class LeaderboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructeur injectant le contexte
        public LeaderboardController(ApplicationDbContext context)
        {
            _context = context;
            LogInitialization();
        }

        // Méthode pour consigner l'initialisation du contrôleur
        private void LogInitialization()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("LeaderboardController initialized");
            Console.ResetColor();
        }

        // Point d'entrée pour récupérer le leaderboard
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLeaderboard()
        {
            LogEndpointCall();

            // Récupérer le leaderboard depuis la base de données
            var leaderboard = await GetLeaderboardFromDb();

            // Optionnel : Utiliser les informations du JWT (ex. l'ID de l'utilisateur connecté)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Récupérer l'ID de l'utilisateur authentifié
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Request made by user ID: {userId}");
            Console.ResetColor();

            return Ok(leaderboard);
        }

        // Méthode pour consigner l'appel à l'endpoint
        private void LogEndpointCall()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("GetLeaderboard endpoint called");
            Console.ResetColor();
        }

        // Méthode pour récupérer les données du leaderboard depuis la base de données
        private async Task<IEnumerable<object>> GetLeaderboardFromDb()
        {
            return await _context.Users
                .OrderByDescending(u => u.Score)
                .Select(u => new { u.Username, u.Score })
                .ToListAsync();
        }
    }
}
