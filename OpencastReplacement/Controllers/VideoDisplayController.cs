using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OpencastReplacement.Data;
using OpencastReplacement.Models;

namespace OpencastReplacement.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private IWebHostEnvironment hostingEnv;
        private ILogger<VideoController> logger;
        private IMongoConnection connection;
        public VideoController(IWebHostEnvironment env, ILogger<VideoController> log, IMongoConnection conn)
        {
            hostingEnv = env;
            logger = log;
            connection = conn;
        }

        [HttpGet]
        public IActionResult Get(string id)
        {
            var coll = connection.Client.GetDatabase("videoserver").GetCollection<Video>("videos");
            var filter = Builders<Video>.Filter.Eq("_id", id);
            var res = coll.FindSync(filter);
            var vid = res.FirstOrDefault();

            string title = vid.FileName;
            string pathToVideo = $"/uploads/{vid.FileName}";
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
