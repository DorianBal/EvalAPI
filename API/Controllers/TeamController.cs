using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;
namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            var teams = await _context.Teams
                .Include(t => t.User)
                .ToListAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔍 Teams retrieved successfully");
            Console.ResetColor();
            
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _context.Teams
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null) return NotFound();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔍 Team retrieved successfully");
            Console.ResetColor();
            return Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult<Team>> CreateTeam(Team team)
        {
            team.CreatedAt = DateTime.Now;
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("👥 Team created successfully");
            Console.ResetColor();
            return CreatedAtAction(nameof(GetTeam), new { id = team.TeamId }, team);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, Team updatedTeam)
        {
            if (id != updatedTeam.TeamId) return BadRequest();
            _context.Entry(updatedTeam).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("👥 Team updated successfully");
                Console.ResetColor();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("👥 Team deleted successfully");
            Console.ResetColor();
            return NoContent();
        }

        private bool TeamExists(int id) => _context.Teams.Any(e => e.TeamId == id);
    }
}
