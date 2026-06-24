// File: Controllers/QuestionController.cs (Updated to use CourseName instead of CourseId)
using LMS.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LMS.DTOs;
using System.Text.Json;
using Newtonsoft.Json;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public QuestionController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
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

        private int SafeGetInt(JsonElement json, string propertyName)
        {
            var prop = json.GetProperty(propertyName);
            return prop.ValueKind == JsonValueKind.Number ? prop.GetInt32() : int.Parse(prop.GetString());
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Question_GetAll", conn) { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    using var cmd = new SqlCommand("sp_GetExamWithQuestions", conn) { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Id", id);

        //    await conn.OpenAsync();
        //    using var reader = await cmd.ExecuteReaderAsync();
        //    if (await reader.ReadAsync())
        //        return Ok(ReadRow(reader));

        //    return NotFound();
        //}


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetExamWithQuestions", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ExamId", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return NotFound();

            // Read Exam details (1st result set)
            var exam = new
            {
                examId = reader.GetInt32(reader.GetOrdinal("ExamId")),
                title = reader.GetString(reader.GetOrdinal("Title")),
                examDate = reader.GetDateTime(reader.GetOrdinal("ExamDate")),
                durationMinutes = reader.GetInt32(reader.GetOrdinal("DurationMinutes")),
                createdBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                examinationID = reader.GetInt32(reader.GetOrdinal("ExaminationID")),
                type = reader.GetString(reader.GetOrdinal("ExamType")),
                questions = new List<object>()
            };

            // Move to next result set (Questions)
            await reader.NextResultAsync();

            var questions = new List<object>();
            while (await reader.ReadAsync())
            {
                questions.Add(new
                {
                    questionText = reader["QuestionText"]?.ToString(),
                    optionA = reader["OptionA"]?.ToString(),
                    optionB = reader["OptionB"]?.ToString(),
                    optionC = reader["OptionC"]?.ToString(),
                    optionD = reader["OptionD"]?.ToString(),
                    correctOption = reader["CorrectOption"]?.ToString(),
                    difficultyLevel = reader["DifficultyLevel"]?.ToString(),
                    topic = reader["Topic"]?.ToString()
                });
            }

            // Return combined data
            return Ok(new
            {
                exam.examId,
                exam.title,
                exam.examDate,
                exam.durationMinutes,
                exam.createdBy,
                exam.examinationID,
                exam.type,
                questions
            });
        }

        //[HttpPost("CreateFull")]
        //public async Task<IActionResult> CreateExamWithQuestions([FromBody] ExamCreateDto dto,
        //            [FromForm] IFormFile file, [FromForm] string examJson)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("File is empty.");

        //    var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "course-content");
        //    Directory.CreateDirectory(uploadsPath);

        //    var originalFileName = Path.GetFileName(file.FileName); // Keep exact original name
        //    var filePath = Path.Combine(uploadsPath, originalFileName);
        //    var fileUrl = $"/uploads/course-content/{originalFileName}";

        //    try
        //    {
        //        using var stream = new FileStream(filePath, FileMode.Create); // Overwrites if exists
        //        await file.CopyToAsync(stream);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "File save failed: " + ex.Message);
        //    } 


        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    await conn.OpenAsync();

        //    using var cmd = new SqlCommand("sp_InsertExamWithQuestions", conn)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };

        //    cmd.Parameters.AddWithValue("@Title", dto.Title);
        //    cmd.Parameters.AddWithValue("@ExamDate", dto.ExamDate);
        //    cmd.Parameters.AddWithValue("@DurationMinutes", dto.DurationMinutes);
        //    cmd.Parameters.AddWithValue("@CreatedBy", dto.CreatedBy);
        //    cmd.Parameters.AddWithValue("@ExaminationID", dto.ExaminationID);
        //    cmd.Parameters.AddWithValue("@ExamType", dto.Type);
        //    cmd.Parameters.AddWithValue("@fileurl", "");

        //    // ✅ Only add question table if any questions are present
        //    if (dto.Questions != null && dto.Questions.Count > 0)
        //    {
        //        var questionTable = new DataTable();
        //        questionTable.Columns.Add("QuestionText");
        //        questionTable.Columns.Add("OptionA");
        //        questionTable.Columns.Add("OptionB");
        //        questionTable.Columns.Add("OptionC");
        //        questionTable.Columns.Add("OptionD");
        //        questionTable.Columns.Add("CorrectOption");
        //        questionTable.Columns.Add("DifficultyLevel");
        //        questionTable.Columns.Add("Topic");

        //        foreach (var q in dto.Questions)
        //        {
        //            questionTable.Rows.Add(
        //                q.QuestionText,
        //                q.OptionA,
        //                q.OptionB,
        //                q.OptionC,
        //                q.OptionD,
        //                q.CorrectOption,
        //                q.DifficultyLevel ?? "Medium",
        //                q.Topic ?? "General"
        //            );
        //        }

        //        var tvp = new SqlParameter("@Questions", questionTable)
        //        {
        //            SqlDbType = SqlDbType.Structured,
        //            TypeName = "QuestionTableType"
        //        };
        //        cmd.Parameters.Add(tvp);
        //    }

        //    var outputId = new SqlParameter("@NewExamId", SqlDbType.Int)
        //    {
        //        Direction = ParameterDirection.Output
        //    };
        //    cmd.Parameters.Add(outputId);

        //    await cmd.ExecuteNonQueryAsync();

        //    return Ok(new { message = "Exam created successfully", examId = (int)outputId.Value });
        //}

        [HttpPost("CreateFull")]
        public async Task<IActionResult> CreateExamWithQuestions(
                    [FromForm] IFormFile file,
                    [FromForm] string examJson)
        {
            string fileUrl = null;
            Console.WriteLine("📥 File received: " + file?.FileName);
            Console.WriteLine("📦 File length: " + file?.Length);

            ExamCreateDto dto;
            try
            { 
                dto = Newtonsoft.Json.JsonConvert.DeserializeObject<ExamCreateDto>(examJson);

                // dto = JsonSerializer.Deserialize<ExamCreateDto>(examJson);
            }
            catch (Exception ex)
            {
                return BadRequest("❌ Failed to parse examJson: " + ex.Message);
            }
             

            if (file != null && file.Length > 0)
            {
                var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "exams");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);
                fileUrl = $"/uploads/exams/{fileName}";

                try
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "❌ File save failed: " + ex.Message);
                }
            }


            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_InsertExamWithQuestions", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Title", dto.Title);
            //cmd.Parameters.AddWithValue("@ExamDate", dto.ExamDate);

            if (DateTime.TryParse(dto.ExamDate, out var parsedDate))
                cmd.Parameters.AddWithValue("@ExamDate", parsedDate);
            else
                return BadRequest("❌ Invalid exam date format.");


            cmd.Parameters.AddWithValue("@DurationMinutes", dto.DurationMinutes);
            cmd.Parameters.AddWithValue("@totmrk", dto.totalmarks);
            cmd.Parameters.AddWithValue("@passmrk", dto.passingmarks);
            cmd.Parameters.AddWithValue("@CreatedBy", dto.CreatedBy);
            cmd.Parameters.AddWithValue("@ExaminationID", dto.ExaminationID);
            cmd.Parameters.AddWithValue("@ExamType", dto.Type);
            //cmd.Parameters.AddWithValue("@fileurl", fileUrl);
            cmd.Parameters.AddWithValue("@fileurl", (object?)fileUrl ?? DBNull.Value);

            if (dto.Questions != null && dto.Questions.Count > 0)
            {
                var questionTable = new DataTable();
                questionTable.Columns.Add("QuestionText");
                questionTable.Columns.Add("OptionA");
                questionTable.Columns.Add("OptionB");
                questionTable.Columns.Add("OptionC");
                questionTable.Columns.Add("OptionD");
                questionTable.Columns.Add("CorrectOption");
                questionTable.Columns.Add("DifficultyLevel");
                questionTable.Columns.Add("Topic");
                questionTable.Columns.Add("SuggestedMarks");

                foreach (var q in dto.Questions)
                {
                    questionTable.Rows.Add(
                        q.QuestionText,
                        q.OptionA,
                        q.OptionB,
                        q.OptionC,
                        q.OptionD,
                        q.CorrectOption,
                        q.DifficultyLevel ?? "Medium",
                        q.Topic ?? "General",
                        q.SuggestedMarks
                    );
                }

                var tvp = new SqlParameter("@Questions", questionTable)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "QuestionTableType"
                };
                cmd.Parameters.Add(tvp);
            }

            var outputId = new SqlParameter("@NewExamId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputId);

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "❌ SQL Execution Failed: " + ex.Message);
            }

            return Ok(new
            {
                message = "✅ Exam created successfully",
                examId = (int)outputId.Value,
                fileUrl
            });
        }

        [HttpGet("GetWithQuestions/{examId}")]
        public async Task<IActionResult> GetExamWithQuestions(int examId)
        {
            var exam = new ExamWithQuestionsDto();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetExamWithQuestions", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ExamId", examId);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            // Read exam details (1st result set)
            if (await reader.ReadAsync())
            {
                exam.ExamId = reader.GetInt32(reader.GetOrdinal("ExamId"));
                exam.Title = reader["Title"]?.ToString();
                exam.ExamDate = reader["ExamDate"]?.ToString();
                exam.DurationMinutes = reader.GetInt32(reader.GetOrdinal("DurationMinutes"));
                exam.CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy"));
                exam.ExaminationID = reader.GetInt32(reader.GetOrdinal("ExaminationID"));
                exam.Type = reader["ExamType"]?.ToString();
            }

            // Move to second result set (questions)
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var question = new QuestionDto
                    {
                        QuestionText = reader["QuestionText"]?.ToString(),
                        OptionA = reader["OptionA"]?.ToString(),
                        OptionB = reader["OptionB"]?.ToString(),
                        OptionC = reader["OptionC"]?.ToString(),
                        OptionD = reader["OptionD"]?.ToString(),
                        CorrectOption = reader["CorrectOption"]?.ToString(),
                        DifficultyLevel = reader["DifficultyLevel"]?.ToString(),
                        Topic = reader["Topic"]?.ToString()
                    };

                    exam.Questions.Add(question);
                }
            }

            return Ok(exam);
        } 

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] object q)
        {
            var json = (JsonElement)q;

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Question_Create", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Subject", json.GetProperty("subject").GetString());
            cmd.Parameters.AddWithValue("@Topic", json.GetProperty("topic").GetString());
            cmd.Parameters.AddWithValue("@QuestionText", json.GetProperty("questionText").GetString());
            cmd.Parameters.AddWithValue("@OptionA", json.GetProperty("optionA").GetString());
            cmd.Parameters.AddWithValue("@OptionB", json.GetProperty("optionB").GetString());
            cmd.Parameters.AddWithValue("@OptionC", json.GetProperty("optionC").GetString());
            cmd.Parameters.AddWithValue("@OptionD", json.GetProperty("optionD").GetString());
            cmd.Parameters.AddWithValue("@CorrectOption", json.GetProperty("correctOption").GetString());
            cmd.Parameters.AddWithValue("@DifficultyLevel", json.GetProperty("difficultyLevel").GetString());
            cmd.Parameters.AddWithValue("@Type", json.GetProperty("type").GetString());
            cmd.Parameters.AddWithValue("@ProgrammeName", json.GetProperty("programmeName").GetString());
            cmd.Parameters.AddWithValue("@CourseName", json.GetProperty("courseName").GetString());
            cmd.Parameters.AddWithValue("@SemesterId", SafeGetInt(json, "semesterId"));
            cmd.Parameters.AddWithValue("@CreatedBy", SafeGetInt(json, "createdBy"));

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Ok(ReadRow(reader));

            return BadRequest("Insert failed");
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] QuestionDto q)
        //{
        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    using var cmd = new SqlCommand("sp_Question_Update", conn) { CommandType = CommandType.StoredProcedure };

        //    cmd.Parameters.AddWithValue("@Id", id);
        //    cmd.Parameters.AddWithValue("@Subject", q.Subject);
        //    cmd.Parameters.AddWithValue("@Topic", q.Topic);
        //    cmd.Parameters.AddWithValue("@QuestionText", q.QuestionText);
        //    cmd.Parameters.AddWithValue("@OptionA", q.OptionA);
        //    cmd.Parameters.AddWithValue("@OptionB", q.OptionB);
        //    cmd.Parameters.AddWithValue("@OptionC", q.OptionC);
        //    cmd.Parameters.AddWithValue("@OptionD", q.OptionD);
        //    cmd.Parameters.AddWithValue("@CorrectOption", q.CorrectOption);
        //    cmd.Parameters.AddWithValue("@DifficultyLevel", q.DifficultyLevel);
        //    cmd.Parameters.AddWithValue("@Type", q.Type);
        //    cmd.Parameters.AddWithValue("@ProgrammeName", q.ProgrammeName);
        //    cmd.Parameters.AddWithValue("@CourseName", q.CourseName);
        //    cmd.Parameters.AddWithValue("@SemesterId", q.SemesterId);
        //    cmd.Parameters.AddWithValue("@CreatedBy", q.CreatedBy);

        //    await conn.OpenAsync();
        //    await cmd.ExecuteNonQueryAsync();
        //    return NoContent();
        //}
        [HttpGet("by-student/{userId}")]
        public async Task<IActionResult> GetByStudent(int userId)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            using var cmd = new SqlCommand(@"
                SELECT q.* FROM Questions q
                INNER JOIN Users u ON q.ProgrammeName = u.Programme
                WHERE u.UserId = @UserId
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        [HttpGet("GetStudentExamWithQuestions/{examId}")]
        public async Task<IActionResult> GetStudentExamWithQuestions(int examId)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetStudentExamWithQuestions", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ExamId", examId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }


        [HttpGet("GetStudentPracticeExamWithQuestions/{examId}")]
        public async Task<IActionResult> GetStudentPracticeExamWithQuestions(int examId)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetStudentPracticeExamWithQuestions", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ExamId", examId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Question_Delete", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@examId", id);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }

        [HttpPost("CreatePracticeExamsWithQuestions")]
        public async Task<IActionResult> CreatePracticeExamsWithQuestions(
            [FromForm] IFormFile file,
            [FromForm] string examJson)
        {
            string fileUrl = null;
            Console.WriteLine("📥 File received: " + file?.FileName);
            Console.WriteLine("📦 File length: " + file?.Length);

            PracticeExamCreateDto dto;
            try
            {
                dto = Newtonsoft.Json.JsonConvert.DeserializeObject<PracticeExamCreateDto>(examJson);

                // dto = JsonSerializer.Deserialize<ExamCreateDto>(examJson);
            }
            catch (Exception ex)
            {
                return BadRequest("❌ Failed to parse examJson: " + ex.Message);
            }


            if (file != null && file.Length > 0)
            {
                var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "Practiceexams");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);
                fileUrl = $"/uploads/Practiceexams/{fileName}";

                try
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "❌ File save failed: " + ex.Message);
                }
            }


            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_PracticeInsertExamWithQuestions", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UnitId", dto.UnitId);
            cmd.Parameters.AddWithValue("@Title", dto.Title);
            //cmd.Parameters.AddWithValue("@ExamDate", dto.ExamDate);

            if (DateTime.TryParse(dto.ExamDate, out var parsedDate))
                cmd.Parameters.AddWithValue("@ExamDate", parsedDate);
            else
                return BadRequest("❌ Invalid exam date format.");


            cmd.Parameters.AddWithValue("@DurationMinutes", dto.DurationMinutes);
            cmd.Parameters.AddWithValue("@totmrk", dto.totalmarks);
            cmd.Parameters.AddWithValue("@passmrk", dto.passingmarks);
            cmd.Parameters.AddWithValue("@CreatedBy", dto.CreatedBy);
            cmd.Parameters.AddWithValue("@ExaminationID", dto.ExaminationID);
            cmd.Parameters.AddWithValue("@ExamType", dto.Type);
            //cmd.Parameters.AddWithValue("@fileurl", fileUrl);
            cmd.Parameters.AddWithValue("@fileurl", (object?)fileUrl ?? DBNull.Value);

            if (dto.Questions != null && dto.Questions.Count > 0)
            {
                var questionTable = new DataTable();
                questionTable.Columns.Add("QuestionText");
                questionTable.Columns.Add("OptionA");
                questionTable.Columns.Add("OptionB");
                questionTable.Columns.Add("OptionC");
                questionTable.Columns.Add("OptionD");
                questionTable.Columns.Add("CorrectOption");
                questionTable.Columns.Add("DifficultyLevel");
                questionTable.Columns.Add("Topic");
                questionTable.Columns.Add("SuggestedMarks");

                foreach (var q in dto.Questions)
                {
                    questionTable.Rows.Add(
                        q.QuestionText,
                        q.OptionA,
                        q.OptionB,
                        q.OptionC,
                        q.OptionD,
                        q.CorrectOption,
                        q.DifficultyLevel ?? "Medium",
                        q.Topic ?? "General",
                        q.SuggestedMarks
                    );
                }

                var tvp = new SqlParameter("@Questions", questionTable)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "QuestionTableType"
                };
                cmd.Parameters.Add(tvp);
            }

            var outputId = new SqlParameter("@NewExamId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputId);

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "❌ SQL Execution Failed: " + ex.Message);
            }

            return Ok(new
            {
                message = "✅ Practice Exam created successfully",
                examId = (int)outputId.Value,
                fileUrl
            });
        }
    }
}
