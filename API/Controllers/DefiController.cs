using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;   
namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DefiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DefiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Defi>>> GetDefies()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔍 Defies retrieved successfully");
            Console.ResetColor();
            return Ok(await _context.Defis.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Defi>> GetDefi(int id)
        {
            var defi = await _context.Defis.FindAsync(id);
            if (defi == null) return NotFound();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔍 Defi retrieved successfully");
            Console.ResetColor();
            return Ok(defi);
        }

        [HttpPost]
        public async Task<ActionResult<Defi>> CreateDefi(Defi defi)
        {
            defi.CreatedAt = DateTime.Now;
            _context.Defis.Add(defi);
            await _context.SaveChangesAsync();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🏆 Defi created successfully");
            Console.ResetColor();
            return CreatedAtAction(nameof(GetDefi), new { id = defi.DefiId }, defi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDefi(int id, Defi updatedDefi)
        {
            if (id != updatedDefi.DefiId) return BadRequest();
            _context.Entry(updatedDefi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("🏆 Defi updated successfully");
                Console.ResetColor();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DefiExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDefi(int id)
        {
            var defi = await _context.Defis.FindAsync(id);
            if (defi == null) return NotFound();

            _context.Defis.Remove(defi);
            await _context.SaveChangesAsync();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🏆 Defi deleted successfully");
            Console.ResetColor();
            return NoContent();
        }

        private bool DefiExists(int id) => _context.Defis.Any(e => e.DefiId == id);
    }
}
