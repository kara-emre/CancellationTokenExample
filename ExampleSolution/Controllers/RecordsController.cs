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

        // Ýptal edilebilir kayýt çekme
        [HttpGet("fetch/cancelable")]
        public async Task<IActionResult> FetchRecordsCancelableAsync(CancellationToken cancellationToken)
        {
            try
            {
                var records = await GetRecordsAsync(cancellationToken); // CancellationToken'ý geçir
                return Ok(records);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("FetchRecordsCancelableAsync => stop canceltoken");
                return StatusCode(499, "Sorgu iptal edildi."); // 499: Client Closed Request (NGINX)
            }
        }

        // Ýptal edilmeden kayýt çekme
        [HttpGet("fetch/normal")]
        public async Task<IActionResult> FetchRecordsNormalAsync()
        {
            var records = await GetRecordsNormalAsync(); // Ýptal kontrolü olmadan çaðýr
            return Ok(records);
        }

        // Ýptal edilebilir kayýt çeken metot
        private async Task<List<int>> GetRecordsAsync(CancellationToken cancellationToken)
        {
            var records = new List<int>();

            for (int i = 1; i <= 200; i++)
            {
                cancellationToken.ThrowIfCancellationRequested(); // Ýptal kontrolü

                records.Add(i); // Kayýt ekle

                // Performansý simüle etmek için gecikme ekliyoruz
                await Task.Delay(50, cancellationToken); // Gecikme sýrasýnda iptal kontrolü

                // Her 1000 kayýtta bir log mesajý verebiliriz
                if (i % 10 == 0)
                {
                    logger.LogInformation($"Cancel => {i} kayýt iþlendi."); // Log kaydý ekle
                }
            }

            return records;
        }

        // Ýptal edilmeyen kayýt çeken metot
        private async Task<List<int>> GetRecordsNormalAsync()
        {
            var records = new List<int>();

            for (int i = 1; i <= 200; i++)
            {
                records.Add(i); // Ýptal kontrolü olmadan kayýt ekle

                // Performansý simüle etmek için gecikme ekliyoruz
                await Task.Delay(50); // Gecikme sýrasýnda iptal kontrolü yok

                // Her 1000 kayýtta bir log mesajý verebiliriz
                if (i % 10 == 0)
                {
                    logger.LogInformation($"Normal => {i} kayýt iþlendi."); // Log kaydý ekle
                }
            }

            return records;
        }
    }
}
