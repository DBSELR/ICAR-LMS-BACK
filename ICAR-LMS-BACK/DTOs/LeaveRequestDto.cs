public class LeaveRequestDto
{
    public int LeaveRequestId { get; set; }
    public string? Name { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? UserCode { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Status { get; set; }
}
