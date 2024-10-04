using Microsoft.AspNetCore.Mvc;

namespace ExampleSolution.Controllers
{
    [ApiController]
    [Route("api/records")]
    public class RecordsController : ControllerBase
    {
        private readonly ILogger<RecordsController> logger;

        public RecordsController(ILogger<RecordsController> _logger)
        {
            logger = _logger;
        }

        /// <summary>
        /// Retrieves a list of records with optional cancellation support.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to stop the request if needed.</param>
        /// <returns>Returns a list of integer records, or a 499 status if the request was canceled.</returns>
        [HttpGet]
        public async Task<IActionResult> GetRecordsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var records = await FetchRecordsAsync(cancellationToken);
                return Ok(records);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("GetRecordsAsync => stop cancellation");
                return StatusCode(499, "Request was canceled by the client."); // 499: Client Closed Request (NGINX)
            }
        }

        /// <summary>
        /// Fetches records with cancellation support.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to stop the process midway.</param>
        /// <returns>Returns a list of integers representing records.</returns>
        private async Task<List<int>> FetchRecordsAsync(CancellationToken cancellationToken)
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
                    logger.LogInformation($"Cancelable => {i} records processed.");
                }
            }

            return records;
        }

        /// <summary>
        /// Retrieves a list of records without cancellation support.
        /// </summary>
        /// <returns>Returns a list of integer records.</returns>
        [HttpGet("normal")]
        public async Task<IActionResult> GetRecordsNormalAsync()
        {
            var records = await FetchRecordsNormalAsync();
            return Ok(records);
        }

        /// <summary>
        /// Fetches records without cancellation support.
        /// </summary>
        /// <returns>Returns a list of integers representing records.</returns>
        private async Task<List<int>> FetchRecordsNormalAsync()
        {
            var records = new List<int>();

            for (int i = 1; i <= 200; i++)
            {
                records.Add(i); // Add record

                // Simulate performance by adding a delay
                await Task.Delay(50); // No cancellation check during delay

                // Log a message every 10 records
                if (i % 10 == 0)
                {
                    logger.LogInformation($"Normal => {i} records processed.");
                }
            }

            return records;
        }
    }
}
