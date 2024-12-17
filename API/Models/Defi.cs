public class Defi
{
    public int DefiId { get; set; }
    public int? ChallengeId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int? Points { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastUpdated { get; set; } = DateTime.Now;
    
}
