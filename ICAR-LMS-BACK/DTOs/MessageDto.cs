namespace LMS.DTOs
{
    public class SendMessageDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }


    public class MessageDto
        {
            public int Id { get; set; }
            public int SenderId { get; set; }         // ✅ Newly added
            public int ReceiverId { get; set; }       // ✅ Newly added
            public string SenderName { get; set; }
            public string ReceiverName { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }
            public bool IsRead { get; set; }
            public DateTime SentAt { get; set; }
      
    }


}
