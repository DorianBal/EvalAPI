using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamMemberController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamMemberController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamMember>>> GetTeamMembers()
        {
            var teamMembers = await _context.TeamMembers
                .Include(tm => tm.User)  // Changed from UserId to User
                .Include(tm => tm.Team)  // Changed from TeamId to Team
                .ToListAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔍 Team members retrieved successfully");
            Console.ResetColor();
            return Ok(teamMembers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamMember>> GetTeamMember(int id)
        {
            var member = await _context.TeamMembers
                .Include(tm => tm.User)  // Changed from UserId to User
                .Include(tm => tm.Team)  // Changed from TeamId to Team
                .FirstOrDefaultAsync(tm => tm.MemberId == id);

            if (member == null) return NotFound();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔍 Team member retrieved successfully");
            Console.ResetColor();
            return Ok(member);
        } 

        [HttpPost]
        public async Task<ActionResult<TeamMember>> CreateTeamMember(TeamMember member)
        {
            _context.TeamMembers.Add(member);
            await _context.SaveChangesAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("👤 Team member created successfully");
            Console.ResetColor();
            return CreatedAtAction(nameof(GetTeamMember), new { id = member.MemberId }, member);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTeamMember(int id, TeamMember updatedMember)
        {
            var member = await _context.TeamMembers.FindAsync(id);
            if (member == null) return NotFound();

            member.TeamId = updatedMember.TeamId;
            member.UserId = updatedMember.UserId;

            await _context.SaveChangesAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("👤 Team member updated successfully");
            Console.ResetColor();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeamMember(int id)
        {
            var member = await _context.TeamMembers.FindAsync(id);
            if (member == null) return NotFound();

            _context.TeamMembers.Remove(member);
            await _context.SaveChangesAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("👤 Team member deleted successfully");
            Console.ResetColor();
            return NoContent();
        }
    }
}
