//using LMS.Models.DTOs;

//namespace LMS.DTOs
//{
//    public class FeeSummaryDto
//    {
//        public int FeeId { get; set; }
//        public int StudentId { get; set; }
//        public string StudentName { get; set; }
//        public CourseDto Course { get; set; }
//        public string Semester { get; set; }     // ✅ Add this
//        public string Programme { get; set; }
//        public decimal AmountDue { get; set; }
//        public decimal AmountPaid { get; set; }
//        public string FeeStatus { get; set; }
//        public DateTime DueDate { get; set; }
//        public DateTime? PaymentDate { get; set; }
//    }

//    public class PayFeeDto
//    {
//        public int FeeId { get; set; }
//        public decimal Amount { get; set; }
//        public string PaymentMethod { get; set; }
//        public string TransactionId { get; set; }
//    }
//}
using LMS.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs
{
//    public class FeeSummaryDto
//    {
//        public int FeeId { get; set; }
//        public int StudentId { get; set; }
//        public string StudentName { get; set; }
//        public CourseDto Course { get; set; }
//        public int semester { get; set; }
//        public string Programme { get; set; } // e.g. "B.Tech"a
//        public string Batch { get; set; }
//        public int programmeId { get; set; }
//        public int groupId { get; set; }
//        public decimal ProgrammeFee { get; set; } // ✅ Added programme fee
//        public decimal AmountDue { get; set; }
//        public decimal AmountPaid { get; set; }
//        public string FeeStatus { get; set; }
//        public DateTime DueDate { get; set; }
//        public DateTime? PaymentDate { get; set; }

//       // public int semester { get; set; }
//    }

//    public class PayFeeDto
//    {
//        public int FeeId { get; set; }
//        public decimal Amount { get; set; }
//        public string PaymentMethod { get; set; }
//        public string TransactionId { get; set; }
//    }

    public class FeeSummaryDto
    {
        public int FeeId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public string Regno { get; set; }
        public int programmeId { get; set; }
        public int groupId { get; set; }
        public string Batch { get; set; }
        public int semester { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public string FeeStatus { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int Installment { get; set; }

        public int SemesterFeeTemplateId { get; set; }
        public decimal Fee { get; set; }
        public decimal Paid { get; set; }
        public decimal Due { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }

        public string StudentIdd { get; set; }

        public int Hid { get; set; }
        public string FeeHead { get; set; }

        public string Remarks { get; set; }
        public int ftid { get; set; }
        public string course { get; set; }
        public string sname { get; set; }
        public string mobile { get; set; }
    }

    

        public class PayFeeDto
    {
        public int FeeId { get; set; }
        public int StudentID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public int Installment { get; set; }
        public int payHeadID { get; set; }
        public int SemesterFeeTemplateId { get; set; }
    }

    public class SemesterFeeRequest
    {
        public string Batch { get; set; }
        public int? ProgrammeId { get; set; }  
        public int? GroupId { get; set; }

        public int? installment { get; set; }
        public DateTime? DueDate { get; set; }

        public decimal? Amount { get; set; }

        public int? Semester { get; set; }

        public int? FeeHeadId { get; set; }
        
    }

    public class InstallmentFeeRequest
    {
        public string Batch { get; set; }
        public int? ProgrammeId { get; set; }

        public int? Hid { get; set; }
        public int? GroupId { get; set; }
        public int? Installment { get; set; }
        public int? colid { get; set; }

    }


    public sealed class PayFeeItemDto
    {
        public string StudentID { get; set; } = default!;
        public decimal Amount { get; set; }        // amount to pay for this row
        public int Installment { get; set; }
        public string? PaymentMethod { get; set; } // default "Cash"
        public string? TransactionId { get; set; } // client/gen id
        public int payHeadID { get; set; } 
        public int ftid { get; set; }
    }




}