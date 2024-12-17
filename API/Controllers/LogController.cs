using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogController : ControllerBase
    {
        private static List<Log> Logs = new List<Log>
        {
            new Log { LogId = 1, UserId = 1, Action = "Login", Timestamp = DateTime.Now, IpAddress = "192.168.1.1" },
            new Log { LogId = 2, UserId = 2, Action = "Logout", Timestamp = DateTime.Now, IpAddress = "192.168.1.2" }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Log>> GetLogs()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔍 Logs retrieved successfully");
            Console.ResetColor();
            return Ok(Logs);
        }

        [HttpGet("{id}")]
        public ActionResult<Log> GetLog(int id)
        {
            var log = Logs.Find(l => l.LogId == id);
            if (log == null) return NotFound();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔍 Log retrieved successfully");
            Console.ResetColor();
            return Ok(log);
        }

        [HttpPost]
        public ActionResult<Log> CreateLog(Log log)
        {
            log.LogId = Logs.Count + 1;
            log.Timestamp = DateTime.Now;
            Logs.Add(log);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("📜 Log created successfully");
            Console.ResetColor();
            return CreatedAtAction(nameof(GetLog), new { id = log.LogId }, log);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateLog(int id, Log updatedLog)
        {
            var log = Logs.Find(l => l.LogId == id);
            if (log == null) return NotFound();

            log.Action = updatedLog.Action;
            log.IpAddress = updatedLog.IpAddress;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("📜 Log updated successfully");
            Console.ResetColor();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteLog(int id)
        {
            var log = Logs.Find(l => l.LogId == id);
            if (log == null) return NotFound();

            Logs.Remove(log);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("📜 Log deleted successfully");
            Console.ResetColor();
            return NoContent();
        }
    }
}
