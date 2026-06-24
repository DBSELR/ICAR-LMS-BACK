using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace LMS.Services
{
    public class SqlScriptExecutor
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public SqlScriptExecutor(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public async Task ExecuteAllSqlFilesAsync()
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");
            var sqlDir = Path.Combine(_env.ContentRootPath, "Database", "StoredProcedures");

            if (!Directory.Exists(sqlDir))
                return;

            foreach (var file in Directory.GetFiles(sqlDir, "*.sql"))
            {
                Console.WriteLine($"📄 Executing SQL File: {Path.GetFileName(file)}");
                var sql = await File.ReadAllTextAsync(file);

                sql = Regex.Replace(
                    sql, @"^\s*GO\s*[\r\n]+", "",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase
                );

                using var conn = new SqlConnection(connString);
                using var cmd = new SqlCommand(sql, conn);

                try
                {
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ ERROR in file: {Path.GetFileName(file)}");
                    Console.WriteLine(ex.Message);
                    throw; // Let it bubble up for visibility
                }
            }
        }


    }
}
