using System.ComponentModel.DataAnnotations;

public class TeamMember
{
    [Key]
    public int MemberId { get; set; }
    public int? TeamId { get; set; }
    public int? UserId { get; set; }

    public Team? Team { get; set; }
    public User? User { get; set; }
}