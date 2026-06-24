//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using System.Data;
//using LMS.Models;

//namespace LMS.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class TaskBoardController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;
//        private readonly string _connectionString;

//        public TaskBoardController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//            _connectionString = _configuration.GetConnectionString("DefaultConnection");
//        }

//        [HttpGet]
//        public IActionResult GetTasks()
//        {
//            var dt = new DataTable();
//            using var conn = new SqlConnection(_connectionString);
//            using var cmd = new SqlCommand("sp_TaskBoard_GetTasks", conn);
//            cmd.CommandType = CommandType.StoredProcedure;
//            new SqlDataAdapter(cmd).Fill(dt);
//            var result = DataTableConverter.ToDictionaryList(dt);
//            return Ok(result);
//        }

//        [HttpPost]
//        public IActionResult CreateTask([FromBody] TaskItem task)
//        {
//            var dt = new DataTable();
//            using var conn = new SqlConnection(_connectionString);
//            using var cmd = new SqlCommand("sp_TaskBoard_CreateTask", conn);
//            cmd.CommandType = CommandType.StoredProcedure;

//            cmd.Parameters.AddWithValue("@Title", task.Title);
//            cmd.Parameters.AddWithValue("@Description", (object?)task.Description ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@Status", task.Status);
//            cmd.Parameters.AddWithValue("@AssignedToUserId", (object?)task.AssignedToUserId ?? DBNull.Value);

//            new SqlDataAdapter(cmd).Fill(dt);
//            var result = DataTableConverter.ToDictionaryList(dt);
//            return Ok(result);
//        }

//        [HttpPut("{id}")]
//        public IActionResult UpdateTask(int id, [FromBody] TaskItem updated)
//        {
//            var dt = new DataTable();
//            using var conn = new SqlConnection(_connectionString);
//            using var cmd = new SqlCommand("sp_TaskBoard_UpdateTask", conn);
//            cmd.CommandType = CommandType.StoredProcedure;

//            cmd.Parameters.AddWithValue("@Id", id);
//            cmd.Parameters.AddWithValue("@Title", updated.Title);
//            cmd.Parameters.AddWithValue("@Description", (object?)updated.Description ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@Status", updated.Status);
//            cmd.Parameters.AddWithValue("@AssignedToUserId", (object?)updated.AssignedToUserId ?? DBNull.Value);

//            new SqlDataAdapter(cmd).Fill(dt);
//            var result = DataTableConverter.ToDictionaryList(dt);
//            return Ok(result);
//        }

//        [HttpDelete("{id}")]
//        public IActionResult DeleteTask(int id)
//        {
//            using var conn = new SqlConnection(_connectionString);
//            using var cmd = new SqlCommand("sp_TaskBoard_DeleteTask", conn);
//            cmd.CommandType = CommandType.StoredProcedure;
//            cmd.Parameters.AddWithValue("@Id", id);

//            conn.Open();
//            cmd.ExecuteNonQuery();
//            return NoContent();
//        }
//    }

//    public static class DataTableConverter
//    {
//        public static List<Dictionary<string, object>> ToDictionaryList(DataTable table)
//        {
//            var list = new List<Dictionary<string, object>>();
//            foreach (DataRow row in table.Rows)
//            {
//                var dict = new Dictionary<string, object>();
//                foreach (DataColumn col in table.Columns)
//                {
//                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
//                }
//                list.Add(dict);
//            }
//            return list;
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using LMS.Models;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskBoardController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public TaskBoardController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTasks()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_TaskBoard_GetTasks", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            new SqlDataAdapter(cmd).Fill(dt);
            var result = DataTableConverter.ToDictionaryList(dt);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateTask([FromBody] TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                return BadRequest("Title is required.");

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_TaskBoard_CreateTask", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Title", task.Title);
            cmd.Parameters.AddWithValue("@Description", (object?)task.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", task.Status);
            cmd.Parameters.AddWithValue("@AssignedToUserId", (object?)task.AssignedToUserId ?? DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateTask(int id, [FromBody] TaskItem updated)
        {
            if (string.IsNullOrWhiteSpace(updated.Title))
                return BadRequest("Title is required.");

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_TaskBoard_UpdateTask", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Title", updated.Title);
            cmd.Parameters.AddWithValue("@Description", (object?)updated.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", updated.Status);
            cmd.Parameters.AddWithValue("@AssignedToUserId", (object?)updated.AssignedToUserId ?? DBNull.Value);

            conn.Open();
            int affectedRows = cmd.ExecuteNonQuery();
            if (affectedRows == 0)
                return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteTask(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_TaskBoard_DeleteTask", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            int affectedRows = cmd.ExecuteNonQuery();
            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }
    }

    public static class DataTableConverter
    {
        public static List<Dictionary<string, object>> ToDictionaryList(DataTable table)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                list.Add(dict);
            }
            return list;
        }
    }
}
