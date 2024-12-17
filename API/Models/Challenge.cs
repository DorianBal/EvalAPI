public class Challenge
{
    public int ChallengeId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int? TotalPoints { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool Status { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public required List<Defi> Defis { get; set; }
    
}
