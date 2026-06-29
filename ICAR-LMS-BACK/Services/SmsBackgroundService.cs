using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LMS_SA_BACK.Services
{
    public class SmsBackgroundService : BackgroundService // Inherit from BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly SmsService _smsService;

        public SmsBackgroundService(IConfiguration config, SmsService smsService)
        {
            _config = config;
            _smsService = smsService;
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        List<(long Id, string Mobile, string Message)> batch = new();

        //        using (var con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        //        using (var cmd = new SqlCommand("SP_GetPendingSms", con))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@BatchSize", 300);

        //            await con.OpenAsync();
        //            using var rdr = await cmd.ExecuteReaderAsync();

        //            while (await rdr.ReadAsync())
        //            {
        //                batch.Add((
        //                    rdr.GetInt64(0),
        //                    rdr.GetString(1),
        //                    rdr.GetString(2)
        //                ));
        //            }
        //        }

        //        if (!batch.Any())
        //        {
        //            await Task.Delay(5000, stoppingToken);
        //            continue;
        //        }

        //        using var semaphore = new SemaphoreSlim(10); // SAFE LIMIT
        //        var tasks = batch.Select(async sms =>
        //        {
        //            await semaphore.WaitAsync();
        //            try
        //            {
        //                bool sent = await _smsService.SendAsync(sms.Mobile, sms.Message);
        //                await UpdateStatus(sms.Id, sent ? "Sent" : "Failed");
        //            }
        //            finally
        //            {
        //                semaphore.Release();
        //            }
        //        });

        //        await Task.WhenAll(tasks);
        //    }
        //}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                List<(long Id, string Mobile, string Message)> batch = new();

                using (var con = new SqlConnection(
                    _config.GetConnectionString("DefaultConnection")))
                using (var cmd = new SqlCommand("SP_GetPendingSms", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BatchSize", 300);

                    await con.OpenAsync(stoppingToken);
                    using var rdr = await cmd.ExecuteReaderAsync(stoppingToken);

                    while (await rdr.ReadAsync(stoppingToken))
                    {
                        batch.Add((
                            rdr.GetInt64(0),
                            rdr.GetString(1),
                            rdr.GetString(2)
                        ));
                    }
                }

                if (!batch.Any())
                {
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                using var semaphore = new SemaphoreSlim(10);

                var tasks = batch.Select(async sms =>
                {
                    await semaphore.WaitAsync(stoppingToken);
                    try
                    {
                        bool sent = await _smsService.SendAsync(sms.Mobile, sms.Message);
                        await UpdateStatus(sms.Id, sent ? "Sent" : "Failed");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks);
            }
        }


        private async Task UpdateStatus(long id, string status)
        {
            using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_UpdateSmsStatus", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Status", status);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
