
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AdminSummaryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminSummaryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private Dictionary<string, object> ReadRow(SqlDataReader reader)
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var camel = char.ToLowerInvariant(name[0]) + name.Substring(1);
                row[camel] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            return row;
        }

        //[HttpGet("dashboard")]
        //public async Task<IActionResult> GetDashboardSummary()
        //{
        //    try
        //    {
        //        var summary = new
        //        {
        //            Students = 0,
        //            Users = 0,
        //            Professors = 0,
        //            Programmes = 0,
        //            Books = 0,
        //            Exams = 0,
        //            Assignments = 0,
        //            LiveClasses = 0,
        //            Tasks = 0,
        //            Leaves = 0,
        //            ContentReadPercentPerBatch = 0m,
        //            liveClassAttendancePercentPerBatch = 0m,
        //            ObjectiveExamAttendancePercentPerBatch = 0m,
        //            SubjectiveExamAttendancePercentPerBatch = 0m
        //        };

        //        using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //        using (var cmd = new SqlCommand("sp_AdminSummary_GetDashboard", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            await conn.OpenAsync();

        //            using (var reader = await cmd.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    summary = new
        //                    {
        //                        Students = Convert.ToInt32(reader["StudentCount"]),
        //                        Users = Convert.ToInt32(reader["UsersCount"]),
        //                        Professors = Convert.ToInt32(reader["ProfessorCount"]),
        //                        Programmes = Convert.ToInt32(reader["ProgrammeCount"]),
        //                        Books = Convert.ToInt32(reader["BookCount"]),
        //                        Exams = Convert.ToInt32(reader["ExamCount"]),
        //                        Assignments = Convert.ToInt32(reader["AssignmentCount"]),
        //                        LiveClasses = Convert.ToInt32(reader["LiveClassCount"]),
        //                        Tasks = Convert.ToInt32(reader["TaskCount"]),
        //                        Leaves = Convert.ToInt32(reader["LeaveCount"]),
        //                        ContentReadPercentPerBatch =
        //                            reader["ContentReadPercentPerBatch"] == DBNull.Value
        //                                ? 0m
        //                                : Convert.ToDecimal(reader["ContentReadPercentPerBatch"]),
        //                        liveClassAttendancePercentPerBatch =
        //                            reader["LiveClassAttendancePercentPerBatch"] == DBNull.Value
        //                                ? 0m
        //                                : Convert.ToDecimal(reader["LiveClassAttendancePercentPerBatch"]),
        //                        ObjectiveExamAttendancePercentPerBatch =
        //                            reader["ObjectiveExamAttendancePercentPerBatch"] == DBNull.Value
        //                                ? 0m
        //                                : Convert.ToDecimal(reader["ObjectiveExamAttendancePercentPerBatch"]),
        //                        SubjectiveExamAttendancePercentPerBatch =
        //                            reader["SubjectiveExamAttendancePercentPerBatch"] == DBNull.Value
        //                                ? 0m
        //                                : Convert.ToDecimal(reader["SubjectiveExamAttendancePercentPerBatch"])
        //                    };
        //                }
        //            }
        //        }

        //        return Ok(summary);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            error = "Failed to fetch dashboard summary",
        //            details = ex.Message
        //        });
        //    }
        //}

        // Models/StudentApprovalSummary.cs
        public class StudentApprovalSummary
        {
            public int total { get; set; }
            public int Approved { get; set; }
            public int pending { get; set; }
        }

        // Models/DashboardSummary.cs
        // Models/DashboardSummary.cs
        public class DashboardSummary
        {
            public int Students { get; set; }
            public int Users { get; set; }
            public int Professors { get; set; }
            public int Programmes { get; set; }
            public int Books { get; set; }
            public int Exams { get; set; }
            public int Assignments { get; set; }
            public int LiveClasses { get; set; }
            public int Tasks { get; set; }
            public int Leaves { get; set; }

            public StudentApprovalSummary StudentApprovalSummary { get; set; }

            public decimal ContentReadPercentPerBatch { get; set; }
            public decimal liveClassAttendancePercentPerBatch { get; set; }
            public decimal ObjectiveExamAttendancePercentPerBatch { get; set; }
            public decimal SubjectiveExamAttendancePercentPerBatch { get; set; }

           
            public decimal TotalFeeAmount { get; set; }
            public decimal TotalPaidAmount { get; set; }
            public decimal TotalDueAmount { get; set; }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                // default instance (optional defaults)
                var summary = new DashboardSummary
                {
                    Students = 0,
                    Users = 0,
                    Professors = 0,
                    Programmes = 0,
                    Books = 0,
                    Exams = 0,
                    Assignments = 0,
                    LiveClasses = 0,
                    Tasks = 0,
                    Leaves = 0,
                    StudentApprovalSummary = new StudentApprovalSummary
                    {
                        total = 0,
                        Approved = 0,
                        pending = 0
                    },
                    ContentReadPercentPerBatch = 0m,
                    liveClassAttendancePercentPerBatch = 0m,
                    ObjectiveExamAttendancePercentPerBatch = 0m,
                    SubjectiveExamAttendancePercentPerBatch = 0m
                };

                using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                using (var cmd = new SqlCommand("sp_AdminSummary_GetDashboard", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            summary.Students = Convert.ToInt32(reader["StudentCount"]);
                            summary.Users = Convert.ToInt32(reader["UsersCount"]);
                            summary.Professors = Convert.ToInt32(reader["ProfessorCount"]);
                            summary.Programmes = Convert.ToInt32(reader["ProgrammeCount"]);
                            summary.Books = Convert.ToInt32(reader["BookCount"]);
                            summary.Exams = Convert.ToInt32(reader["ExamCount"]);
                            summary.Assignments = Convert.ToInt32(reader["AssignmentCount"]);
                            summary.LiveClasses = Convert.ToInt32(reader["LiveClassCount"]);
                            summary.Tasks = Convert.ToInt32(reader["TaskCount"]);
                            summary.Leaves = Convert.ToInt32(reader["LeaveCount"]);

                            summary.StudentApprovalSummary = new StudentApprovalSummary
                            {
                                total = Convert.ToInt32(reader["TotalStudentsCount"]),
                                Approved = Convert.ToInt32(reader["ApprovedStudentsCount"]),
                                pending = Convert.ToInt32(reader["ToBeApprovedStudentsCount"])
                            };

                            summary.ContentReadPercentPerBatch = reader["ContentReadPercentPerBatch"] != DBNull.Value
                                ? Convert.ToDecimal(reader["ContentReadPercentPerBatch"])
                                : 0m;
                            summary.liveClassAttendancePercentPerBatch = reader["LiveClassAttendancePercentPerBatch"] != DBNull.Value
                                ? Convert.ToDecimal(reader["LiveClassAttendancePercentPerBatch"])
                                : 0m;
                            summary.ObjectiveExamAttendancePercentPerBatch = reader["ObjectiveExamAttendancePercentPerBatch"] != DBNull.Value
                                ? Convert.ToDecimal(reader["ObjectiveExamAttendancePercentPerBatch"])
                                : 0m;
                            summary.SubjectiveExamAttendancePercentPerBatch = reader["SubjectiveExamAttendancePercentPerBatch"] != DBNull.Value
                                ? Convert.ToDecimal(reader["SubjectiveExamAttendancePercentPerBatch"])
                                : 0m;

                         
                            summary.TotalFeeAmount = reader["TotalFeeAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalFeeAmount"]) : 0m;
                            summary.TotalPaidAmount = reader["TotalPaidAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPaidAmount"]) : 0m;
                            summary.TotalDueAmount = reader["TotalDueAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalDueAmount"]) : 0m;
                        }
                    }
                }

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Failed to fetch dashboard summary",
                    details = ex.Message
                });
            }
        }


        [HttpGet("AppGenesisdashboard")]
        public async Task<IActionResult> GetAppGenesisdashboard()
        {
            try
            {
                var summary = new
                {
                    studentsCount = 0,
                    approved = 0,
                    tobeApproved = 0
                };

                using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                using (var cmd = new SqlCommand("sp_AppGenesis_GetDashboard", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            summary = new
                            {
                                studentsCount = Convert.ToInt32(reader["studentsCount"]),
                                approved = Convert.ToInt32(reader["approved"]),
                                tobeApproved = Convert.ToInt32(reader["tobeApproved"])
                            };
                        }
                    }
                }

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Failed to fetch AppGenesis dashboard summary",
                    details = ex.Message
                });
            }
        }

        [HttpGet("GetFeesummary")]
        public async Task<IActionResult> GetFeesummary()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetFeesummary", conn)
            { CommandType = CommandType.StoredProcedure };
            //cmd.Parameters.AddWithValue("@InstructorId", InstructorId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }



        //[HttpGet("dashboard")]
        //public async Task<IActionResult> GetDashboardSummary()
        //{
        //    try
        //    {
        //        var summary = new
        //        {
        //            Students = 0,
        //            Users = 0,
        //            Professors = 0,
        //            Programmes = 0,
        //            Books = 0,
        //            Exams = 0,
        //            Assignments = 0,
        //            LiveClasses = 0,
        //            Tasks = 0,
        //            Leaves = 0
        //        };

        //        using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //        using (var cmd = new SqlCommand("sp_AdminSummary_GetDashboard", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            await conn.OpenAsync();
        //            var reader = await cmd.ExecuteReaderAsync();
        //            if (await reader.ReadAsync())
        //            {
        //                summary = new
        //                {
        //                    Students = Convert.ToInt32(reader["StudentCount"]),
        //                    Users = Convert.ToInt32(reader["UsersCount"]),
        //                    Professors = Convert.ToInt32(reader["ProfessorCount"]),
        //                    Programmes = Convert.ToInt32(reader["ProgrammeCount"]),
        //                    Books = Convert.ToInt32(reader["BookCount"]),
        //                    Exams = Convert.ToInt32(reader["ExamCount"]),
        //                    Assignments = Convert.ToInt32(reader["AssignmentCount"]),
        //                    LiveClasses = Convert.ToInt32(reader["LiveClassCount"]),
        //                    Tasks = Convert.ToInt32(reader["TaskCount"]),
        //                    Leaves = Convert.ToInt32(reader["LeaveCount"])
        //                };
        //            }
        //        }

        //        return Ok(summary);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = "Failed to fetch dashboard summary", details = ex.Message });
        //    }
        //}
    }
}
