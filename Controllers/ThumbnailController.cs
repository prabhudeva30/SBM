using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThumbnailController : ControllerBase
    {
        private readonly ILogger<ThumbnailController> _logger;
        private readonly IThumbnailService _thumbnailService;

        public ThumbnailController(ILogger<ThumbnailController> logger, IThumbnailService thumbnailService)
        {
            _logger = logger;
            _thumbnailService = thumbnailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllThumbnails()
        {
            _logger.LogInformation("Received request to get all thumbnails.");
            var thumbnails = await _thumbnailService.GetAllThumbnailsAsync();
            _logger.LogInformation("Successfully retrieved {Count} thumbnails.", thumbnails.ImageShape.Count);
            return Ok(thumbnails);
        }
    }
}