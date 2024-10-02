using Microsoft.AspNetCore.Mvc;

namespace ExampleSolution.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordsController : ControllerBase
    {
        private readonly ILogger<RecordsController> logger;

        public RecordsController(ILogger<RecordsController> _logger)
        {
            logger = _logger;
        }

        // Fetch records that can be canceled
        [HttpGet("fetch/cancelable")]
        public async Task<IActionResult> FetchRecordsCancelableAsync(CancellationToken cancellationToken)
        {
            try
            {
                var records = await GetRecordsAsync(cancellationToken); // Pass the CancellationToken
                return Ok(records);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("FetchRecordsCancelableAsync => stop canceltoken");
                return StatusCode(499, "Query canceled."); // 499: Client Closed Request (NGINX)
            }
        }

        // Fetch records without cancellation
        [HttpGet("fetch/normal")]
        public async Task<IActionResult> FetchRecordsNormalAsync()
        {
            var records = await GetRecordsNormalAsync(); // Call without cancellation check
            return Ok(records);
        }

        // Method to fetch records that can be canceled
        private async Task<List<int>> GetRecordsAsync(CancellationToken cancellationToken)
        {
            var records = new List<int>();

            for (int i = 1; i <= 200; i++)
            {
                cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation

                records.Add(i); // Add record

                // Simulate performance by adding a delay
                await Task.Delay(50, cancellationToken); // Check for cancellation during delay

                // Log a message every 10 records
                if (i % 10 == 0)
                {
                    logger.LogInformation($"Cancel => {i} records processed."); // Add log entry
                }
            }

            return records;
        }

        // Method to fetch records without cancellation
        private async Task<List<int>> GetRecordsNormalAsync()
        {
            var records = new List<int>();

            for (int i = 1; i <= 200; i++)
            {
                records.Add(i); // Add record without cancellation check

                // Simulate performance by adding a delay
                await Task.Delay(50); // No cancellation check during delay

                // Log a message every 10 records
                if (i % 10 == 0)
                {
                    logger.LogInformation($"Normal => {i} records processed."); // Add log entry
                }
            }

            return records;
        }
    }
}
