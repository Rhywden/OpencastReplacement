using Microsoft.AspNetCore.Mvc;

namespace OpencastReplacement.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private IWebHostEnvironment hostingEnv;
        private ILogger<VideoController> logger;
        public VideoController(IWebHostEnvironment env, ILogger<VideoController> log)
        {
            hostingEnv = env;
            logger = log;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string title = "Test";
            string pathToVideo = "/uploads/cymatics.mp4";
            string output = string.Join(System.Environment.NewLine, new string[]
            {
                "<!DOCTYPE html>",
                "<html>",
                "<head>",
                "<meta charset=\"utf-8\">",
                $"<title>{title}</title>",
                "<link rel=\"stylesheet\" href=\"https://cdn.plyr.io/3.6.12/plyr.css\" />",
                "</head>",
                "<body>",
                "<video id=\"player\" playsinline controls data-plyr-config='{ \"autoplay\": true }'>",
                $"<source src=\"{pathToVideo}\" type=\"video/mp4\" />",
                "</video>",
                "<script src=\"https://cdn.plyr.io/3.6.12/plyr.js\"></script>",
                "<script>",
                "const player = new Plyr('#player');",
                "</script>",
                "</body>",
                "</html>"
            });


            return new ContentResult { 
                Content = output,
                ContentType = "text/html"
            };
        }
    }
}
