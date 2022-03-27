using OpencastReplacement.Models;

namespace OpencastReplacement.Services
{
    public interface IFfmpegWrapper
    {
        public Task<bool> StartEncoding(Video video);
        public Task<bool> CancelEncoding(string id);

    }
}
