using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("UserController initialized");
            Console.ResetColor();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("GetUsers endpoint called");
            var users = await _context.Users.ToListAsync();
            Console.WriteLine($"Found {users.Count} users");
            Console.ResetColor();
            
            var userDetails = await Task.WhenAll(users.Select(async user =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Processing user {user.Id}");
                Console.ResetColor();
                
                var teamMemberships = await _context.TeamMembers
                      .Where(tm => tm.UserId == user.Id)
                      .Join(_context.Teams,
                          tm => tm.TeamId,
                          t => t.TeamId,
                          (tm, t) => t)
                    .ToListAsync();


                var participations = await _context.Participations
                    .Where(p => p.User.Id == user.Id)
                    .Include(p => p.Challenge)
                        .ThenInclude(c => c.Defis)
                    .Select(p => new
                    {
                        Challenge = new
                        {
                            p.Challenge.ChallengeId,
                            p.Challenge.Title,
                            p.Challenge.Description,
                            p.Challenge.TotalPoints,
                            p.Challenge.Status,
                            Defis = p.Challenge.Defis.Select(d => new
                            {
                                d.DefiId,
                                d.Title,
                                d.Description,
                                d.Points,
                                d.CreatedAt
                            })
                        },
                        p.ParticipationDate,
                        p.Score
                    })
                    .ToListAsync();

                return new
                {
                    User = new
                    {
                        user.Id,
                        user.Username,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.Score,
                        user.CreatedAt
                    },
                    Teams = teamMemberships,
                    Participations = participations
                };
            }));

            return Ok(userDetails);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"GetUser endpoint called for id: {id}");
            Console.ResetColor();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"User with id {id} not found");
                Console.ResetColor();
                return NotFound();
            }

            var teamMemberships = await _context.TeamMembers
                .Where(tm => tm.UserId == user.Id)
                .Join(_context.Teams,
                    tm => tm.TeamId,
                    t => t.TeamId,
                    (tm, t) => t)
                .ToListAsync();

            var participations = await _context.Participations
                .Where(p => p.User.Id == user.Id)
                .Include(p => p.Challenge)
                    .ThenInclude(c => c.Defis)
                .Select(p => new
                {
                    Challenge = new
                    {
                        p.Challenge.ChallengeId,
                        p.Challenge.Title,
                        p.Challenge.Description,
                        p.Challenge.TotalPoints,
                        p.Challenge.Status,
                        Defis = p.Challenge.Defis.Select(d => new
                        {
                            d.DefiId,
                            d.Title,
                            d.Description,
                            d.Points,
                            d.CreatedAt
                        })
                    },
                    p.ParticipationDate,
                    p.Score
                })
                .ToListAsync();

            var userDetails = new
            {
                User = new
                {
                    user.Id,
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Score,
                    user.CreatedAt
                },
                Teams = teamMemberships,
                Participations = participations
            };

            return Ok(userDetails);
        }

        [HttpGet("{id}/challenges")]
        public async Task<ActionResult> GetUserWithChallenges(int id)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"GetUserWithChallenges endpoint called for id: {id}");
            Console.ResetColor();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"User with id {id} not found");
                Console.ResetColor();
                return NotFound();
            }

            var userParticipations = await _context.Participations
                .Where(p => p.User.Id == id)
                .Include(p => p.Challenge)
                .Select(p => new
                {
                    p.Challenge,
                    p.ParticipationDate,
                    p.Score
                }).ToListAsync();

            var userWithChallenges = new
            {
                User = user,
                Challenges = userParticipations
            };

            return Ok(userWithChallenges);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Creating new user: {user.Username}");
            Console.ResetColor();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Updating user with id: {id}");
            Console.ResetColor();

            if (id != updatedUser.Id) return BadRequest();
            _context.Entry(updatedUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"User with id {id} not found during update");
                    Console.ResetColor();
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Deleting user with id: {id}");
            Console.ResetColor();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"User with id {id} not found for deletion");
                Console.ResetColor();
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UserExists(int id) => _context.Users.Any(e => e.Id == id);
    }
}
