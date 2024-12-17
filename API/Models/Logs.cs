public class Log
{
    public int LogId { get; set; }
    public int? UserId { get; set; }
    public required string Action { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public required string IpAddress { get; set; }
}
