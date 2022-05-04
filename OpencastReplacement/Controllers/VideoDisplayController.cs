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
        private readonly IMongoConnection connection;
        public VideoController(IMongoConnection conn)
        {
            connection = conn;
        }

        [HttpGet]
        public IActionResult Get(string id)
        {
            var coll = connection.GetVideoCollection();
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
