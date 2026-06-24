using LMS.Models;

public class Notification
{
    public int NotificationId { get; set; }
    public string Message { get; set; }
    public string NotificationType { get; set; }
    public DateTime DateSent { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; }
}
