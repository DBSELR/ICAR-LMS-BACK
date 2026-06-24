    // File: Services/FeeService.cs
    using LMS.Data;
    using LMS.DTOs;
    using LMS.Models;
    using LMS.Models.DTOs;
    using Microsoft.EntityFrameworkCore;

    namespace LMS.Services
    {
        public interface IFeeService
        {
            Task<List<FeeSummaryDto>> GetStudentFees(int studentId);
            Task<List<FeeSummaryDto>> GetAllFees();
            Task<bool> PayFee(PayFeeDto dto);
            // Task GenerateSemesterFeesFromTemplate(int studentId);
            Task GenerateSemesterFeeForCurrentSemester(int studentId,string Batch, int programmeId,int groupId, int semester);

        }

        public class FeeService : IFeeService
        {
            private readonly AppDbContext _context;

            public FeeService(AppDbContext context)
            {
                _context = context;
            }

        //public async Task GenerateSemesterFeesFromTemplate(int studentId)
        //{
        //    var student = await _context.Users.FindAsync(studentId);
        //    if (student == null || student.Programme == null) return;

        //    var existingSemesters = await _context.Fees
        //        .Where(f => f.StudentId == studentId)
        //        .Select(f => f.Semester)
        //        .ToListAsync();

        //    var templates = await _context.SemesterFeeTemplate
        //        .Where(t => t.Programme == student.Programme && !existingSemesters.Contains(t.Semester))
        //        .ToListAsync();

        //    foreach (var template in templates)
        //    {
        //        _context.Fees.Add(new Fee
        //        {
        //            StudentId = studentId,
        //            Semester = template.Semester,
        //            Programme = template.Programme,
        //            AmountDue = template.AmountDue,
        //            AmountPaid = 0,
        //            FeeStatus = "Pending",
        //            DueDate = DateTime.UtcNow.AddMonths(1),
        //            CreatedAt = DateTime.UtcNow
        //        });
        //    }

        //    await _context.SaveChangesAsync();
        //}



        public async Task GenerateSemesterFeeForCurrentSemester(int studentId, string Batch, int programmeId, int groupId, int semester)
        {
            //var student = await _context.Users.FindAsync(studentId);
            //if (student == null) return;

            bool alreadyExists = await _context.Fees
                .AnyAsync(f => f.StudentId == studentId && f.programmeId == programmeId && f.groupId == groupId && f.semester == semester);

            if (alreadyExists) return;

            var template = await _context.SemesterFeeTemplate
                .FirstOrDefaultAsync(t => t.Batch == Batch && t.programmeId == programmeId && t.groupId == groupId && t.semester == semester);

            if (template == null) return;

            var fee = new Fee
            {
                StudentId = studentId,
                Batch = template.Batch,
                semester = template.semester,
                programmeId = template.programmeId,
                groupId = template.groupId,
                AmountDue = template.AmountDue,
                AmountPaid = 0,
                FeeStatus = "Pending",
                DueDate = DateTime.UtcNow.AddMonths(1),
                CreatedAt = DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString(), // 💥 REQUIRED: avoid null constraint error
                PaymentMethod = "NA"
            };

            _context.Fees.Add(fee);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FeeSummaryDto>> GetStudentFees(int studentId)
            {
                return await _context.Fees
                    .Include(f => f.Student)
                    .Where(f => f.StudentId == studentId)
                    .Select(f => new FeeSummaryDto
                    {
                        FeeId = f.FeeId,
                        StudentId = f.StudentId,
                        StudentName = f.Student.FirstName + " " + f.Student.LastName,
                        Batch = f.Batch,
                        semester = f.semester,
                        programmeId = f.programmeId,
                        groupId = f.groupId,
                        AmountDue = f.AmountDue,
                        AmountPaid = f.AmountPaid,
                        FeeStatus = f.FeeStatus,
                        DueDate = f.DueDate,
                        PaymentDate = f.PaymentDate
                    })
                    .ToListAsync();
            }

            public async Task<List<FeeSummaryDto>> GetAllFees()
            {
                return await _context.Fees
                    .Include(f => f.Student)
                    .Select(f => new FeeSummaryDto
                    {
                        FeeId = f.FeeId,
                        StudentId = f.StudentId,
                        StudentName = f.Student.FirstName + " " + f.Student.LastName,
                        Batch = f.Batch,
                        semester = f.semester,
                        programmeId = f.programmeId,
                        groupId = f.groupId,
                        AmountDue = f.AmountDue,
                        AmountPaid = f.AmountPaid,
                        FeeStatus = f.FeeStatus,
                        DueDate = f.DueDate
                    })
                    .ToListAsync();
            }

            public async Task<bool> PayFee(PayFeeDto dto)
            {
                var fee = await _context.Fees.FindAsync(dto.FeeId);
                if (fee == null) return false;

                fee.AmountPaid += dto.Amount;
                fee.UpdatedAt = DateTime.UtcNow;
                fee.PaymentMethod = dto.PaymentMethod;
                fee.TransactionId = dto.TransactionId;
                fee.PaymentDate = DateTime.UtcNow;

                if (fee.AmountPaid >= fee.AmountDue)
                    fee.FeeStatus = "Paid";
                else if (fee.AmountPaid > 0)
                    fee.FeeStatus = "PartiallyPaid";

                await _context.SaveChangesAsync();
                return true;
            }
        }
    }