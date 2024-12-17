using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChallengeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChallengeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Challenge>>> GetChallenges()
        {
            var challenges = await _context.Challenges
                .Include(c => c.Defis)
                .ToListAsync();

            if (challenges == null || !challenges.Any()) return NotFound();
            return Ok(challenges);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Challenge>> GetChallenge(int id)
        {
            var challenge = await _context.Challenges
                .Include(c => c.Defis)
                .FirstOrDefaultAsync(c => c.ChallengeId == id);
                
            
            if (challenge == null) return NotFound();
            return Ok(challenge);
        }

        [HttpPost]
        public async Task<ActionResult<Challenge>> CreateChallenge(Challenge challenge)
        {
            _context.Challenges.Add(challenge);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetChallenge), new { id = challenge.ChallengeId }, challenge);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChallenge(int id, Challenge updatedChallenge)
        {
            if (id != updatedChallenge.ChallengeId) return BadRequest();
            _context.Entry(updatedChallenge).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChallengeExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChallenge(int id)
        {
            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge == null) return NotFound();

            _context.Challenges.Remove(challenge);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ChallengeExists(int id) => _context.Challenges.Any(e => e.ChallengeId == id);
    }
}
