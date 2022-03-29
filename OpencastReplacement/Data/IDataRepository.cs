using OpencastReplacement.Models;

namespace OpencastReplacement.Data
{
    public interface IDataRepository
    {
        public List<Video> Videos { get; set; }
        public List<Conversion> Conversions { get; set; }
        public List<Tag> Tags { get; set; }

        public Task Init();
    }
}
