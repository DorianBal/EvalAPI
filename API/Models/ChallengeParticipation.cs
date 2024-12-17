using System.ComponentModel.DataAnnotations;

public class ChallengeParticipation
{
    [Key]
    public int ParticipationId { get; set; }
    public int ChallengeId { get; set; }
    public int UserId { get; set; }
    public DateTime ParticipationDate { get; set; } = DateTime.Now;
    public int Score { get; set; }

    // Navigation properties (optional)
    public required Challenge Challenge { get; set; }
    public required User User { get; set; }
}
