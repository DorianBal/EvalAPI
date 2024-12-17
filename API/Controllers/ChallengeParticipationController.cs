using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChallengeParticipationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChallengeParticipationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChallengeParticipation>>> GetParticipations()
        {
            return Ok(await _context.Participations.Include(p => p.User).Include(p => p.Challenge).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChallengeParticipation>> GetParticipation(int id)
        {
            var participation = await _context.Participations.Include(p => p.User).Include(p => p.Challenge).FirstOrDefaultAsync(p => p.ParticipationId == id);
            if (participation == null) return NotFound();
            return Ok(participation);
        }

        [HttpPost]
        public async Task<ActionResult<ChallengeParticipation>> CreateParticipation(ChallengeParticipation participation)
        {
            _context.Participations.Add(participation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetParticipation), new { id = participation.ParticipationId }, participation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParticipation(int id, ChallengeParticipation updatedParticipation)
        {
            if (id != updatedParticipation.ParticipationId) return BadRequest();
            _context.Entry(updatedParticipation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipationExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipation(int id)
        {
            var participation = await _context.Participations.FindAsync(id);
            if (participation == null) return NotFound();

            _context.Participations.Remove(participation);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ParticipationExists(int id) => _context.Participations.Any(e => e.ParticipationId == id);
    }
}
