using OpencastReplacement.Models;

namespace OpencastReplacement.Data
{
    public interface IDataRepository
    {
        public List<Video> Videos { get; set; }
        public List<Conversion> Conversions { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Series> Series { get; set; }

        public Task Init();

        public void AddVideo(Video vid);
        public void DeleteVideo(Video vid);
        public void UpdateVideo(Video vid);

        public void AddTag(Tag tag);
        public void UpdateTag(Tag tag);
        public void DeleteTag(Tag tag);

        public void AddSeries(Series series);
        public void UpdateSeries(Series series);
        public void DeleteSeries(Series series);
    }
}
