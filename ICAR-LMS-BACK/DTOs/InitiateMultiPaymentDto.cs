namespace LMS.DTOs
{
    public class InitiateMultiPaymentDto
    {
        public string Username { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public List<UserPaymentDto> Payments { get; set; }
    }

}
