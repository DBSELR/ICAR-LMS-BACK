using LMS.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Message
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }

    [ForeignKey("SenderId")]
    public User Sender { get; set; }

    [ForeignKey("ReceiverId")]
    public User Receiver { get; set; }
}
