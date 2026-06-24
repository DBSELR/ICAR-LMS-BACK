using Microsoft.Data.SqlClient;
using System.Data;
using LMS.DTOs;
using Microsoft.Extensions.Logging;

namespace LMS.Services
{
    public class UserPaymentService
    {
        private readonly string _connectionString;
        private readonly ILogger<UserPaymentService> _logger;

        public UserPaymentService(IConfiguration configuration, ILogger<UserPaymentService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Returns number of rows inserted
        public async Task<int> InsertUserPaymentsAsync(
            List<UserPaymentDto> payments,
            string merchantOrderId)
        {
            if (payments == null || payments.Count == 0)
                return 0;

            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            using var tran = con.BeginTransaction();
            try
            {
                foreach (var p in payments)
                {
                    using var cmd = new SqlCommand("SP_InsertUserPayments", con, tran);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", p.UserId);
                    cmd.Parameters.AddWithValue("@Hid", p.Hid);
                    cmd.Parameters.AddWithValue("@Amount", p.Amount);
                    cmd.Parameters.AddWithValue("@MerchantOrderId", merchantOrderId);

                    await cmd.ExecuteNonQueryAsync();
                }

                tran.Commit();
                _logger.LogInformation("Inserted {Count} UserPayments rows for {Order}", payments.Count, merchantOrderId);
                return payments.Count;
            }
            catch (Exception ex)
            {
                try { tran.Rollback(); } catch { }
                _logger.LogError(ex, "Failed to insert UserPayments for {Order}", merchantOrderId);
                throw;
            }
        }

        public async Task MarkUserPaymentsPaidAsync(string merchantOrderId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_MarkUserPaymentsPaid", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MerchantOrderId", merchantOrderId);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
