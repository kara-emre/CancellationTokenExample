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

        // �ptal edilebilir kay�t �ekme
        [HttpGet("fetch/cancelable")]
        public async Task<IActionResult> FetchRecordsCancelableAsync(CancellationToken cancellationToken)
        {
            try
            {
                var records = await GetRecordsAsync(cancellationToken); // CancellationToken'� ge�ir
                return Ok(records);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("FetchRecordsCancelableAsync => stop canceltoken");
                return StatusCode(499, "Sorgu iptal edildi."); // 499: Client Closed Request (NGINX)
            }
        }

        // �ptal edilmeden kay�t �ekme
        [HttpGet("fetch/normal")]
        public async Task<IActionResult> FetchRecordsNormalAsync()
        {
            var records = await GetRecordsNormalAsync(); // �ptal kontrol� olmadan �a��r
            return Ok(records);
        }

        // �ptal edilebilir kay�t �eken metot
        private async Task<List<int>> GetRecordsAsync(CancellationToken cancellationToken)
        {
            var records = new List<int>();

            for (int i = 1; i <= 200; i++)
            {
                cancellationToken.ThrowIfCancellationRequested(); // �ptal kontrol�

                records.Add(i); // Kay�t ekle

                // Performans� sim�le etmek i�in gecikme ekliyoruz
                await Task.Delay(50, cancellationToken); // Gecikme s�ras�nda iptal kontrol�

                // Her 1000 kay�tta bir log mesaj� verebiliriz
                if (i % 10 == 0)
                {
                    logger.LogInformation($"Cancel => {i} kay�t i�lendi."); // Log kayd� ekle
                }
            }

            return records;
        }

        // �ptal edilmeyen kay�t �eken metot
        private async Task<List<int>> GetRecordsNormalAsync()
        {
            var records = new List<int>();

            for (int i = 1; i <= 200; i++)
            {
                records.Add(i); // �ptal kontrol� olmadan kay�t ekle

                // Performans� sim�le etmek i�in gecikme ekliyoruz
                await Task.Delay(50); // Gecikme s�ras�nda iptal kontrol� yok

                // Her 1000 kay�tta bir log mesaj� verebiliriz
                if (i % 10 == 0)
                {
                    logger.LogInformation($"Normal => {i} kay�t i�lendi."); // Log kayd� ekle
                }
            }

            return records;
        }
    }
}
