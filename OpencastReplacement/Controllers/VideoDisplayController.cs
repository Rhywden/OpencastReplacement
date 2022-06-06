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
                "<link rel=\"stylesheet\" href=\"https://vjs.zencdn.net/7.18.1/video-js.css\" />",
                "<style>",
                "body { margin: 0 auto; }",
                "#player {",
                "width: 100vw;",
                "height: 100vh;",
                "}",
                "</style>",
                "</head>",
                "<body>",
                "<video id=\"player\" class=\"video-js\" controls preload=\"auto\" data-setup=\"{}\" fluid liveui >",
                $"<source src=\"{pathToVideo}\" type=\"video/mp4\" />",
                "</video>",
                "<script src=\"https://vjs.zencdn.net/7.18.1/video.min.js\"></script>",
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
