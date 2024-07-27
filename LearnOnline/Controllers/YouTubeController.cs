using Microsoft.AspNetCore.Mvc;
using LearnOnline.Services;

namespace LearnOnline.Controllers
{
    public class YouTubeController : Controller
    {
        private readonly YouTubeApiService _youTubeService;

        public YouTubeController(YouTubeApiService youTubeService)
        {
            _youTubeService = youTubeService;
        }

        public async Task<IActionResult> Index()
        {
            var playlistId = "PLJjeYOy1uzyBlzHarKwFuzlu56S-4fJ92";
            var videoDetails = await _youTubeService.GetVideoDetailsFromPlaylistAsync(playlistId);

            return View(videoDetails);
        }
    }
}
