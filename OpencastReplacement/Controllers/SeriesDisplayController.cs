using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OpencastReplacement.Data;
using OpencastReplacement.Models;
using System.Text.RegularExpressions;

namespace OpencastReplacement.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly IMongoConnection connection;

        public SeriesController(IMongoConnection conn)
        {
            connection = conn;
        }

        [HttpGet]
        public IActionResult Get(string id)
        {
            var collection = connection.GetSeriesCollection();
            var filter = Builders<Series>.Filter.Eq("_id", id);
            var result = collection.FindSync(filter);
            var series = result.FirstOrDefault();



            string output = string.Join(System.Environment.NewLine, new string[]
            {
                "<!DOCTYPE html>",
                "<html>",
                "<head>",
                "<meta charset=\"utf-8\">",
                $"<title>Test</title>",
                "<link rel=\"stylesheet\" href=\"https://vjs.zencdn.net/7.18.1/video-js.css\" />",
                "<link rel=\"stylesheet\" href=\"/css/videojs-playlist-ui.vertical.css\" />",
                "<style>",
                "body { margin: 0 auto; }",
                ".container { display: flex; width: 100vw; height: 100vh; }",
                "#player { width: calc(100% - 300px); height: 100%; }",
                "#playlist { width: 300px; height: 100%; }",
                "</style>",
                "</head>",
                "<body>",
                "<div class=\"container\">",
                "<video id=\"player\" class=\"video-js\" controls preload=\"auto\" data-setup=\"{}\" fluid liveui ></video>",
                "<div class=\"vjs-playlist\" id=\"playlist\"></div>",
                "</div>",
                "<script src=\"https://vjs.zencdn.net/7.18.1/video.min.js\"></script>",
                "<script src=\"js/videojs-playlist.min.js\"></script>",
                "<script src=\"js/videojs-playlist-ui.min.js\"></script>",
                "<script>",
                "var player = videojs('player');",
                "player.playlistUi();",
                "player.playlist([",
            });

            string[] vids = new string[series.Videos.Count];

            for(int i = 0; i < vids.Length; i++)
            {
                string descr = Regex.Replace(series.Videos[i].Beschreibung ?? string.Empty, "[^a-zA-Z0-9ÄÖÜäöü ._,ß()]", string.Empty);
                vids[i] = string.Join(System.Environment.NewLine, new string[]
                {
                    "{\"sources\": [{",

                    $"\"src\": \"/uploads/{series.Videos[i].FileName}\",",
                    "\"type\": \"video/mp4\"",
                    "}],",
                    $"\"name\": \"{series.Videos[i].Titel ?? string.Empty}\",",
                    $"\"description\": \"{descr}\",",
                    "\"poster\": \"/images/BS18.jpg\"",
                    "}"
                });
            }


            string vidsText = string.Join(",", vids);

            output = string.Join(System.Environment.NewLine, new string[]
            {
                output,
                vidsText,
                "]);",
                "</script>",
                "</body>",
                "</html>"
            });
            return new ContentResult
            {
                Content = output,
                ContentType = "text/html"
            };
        }
    }
}
