using OpencastReplacement.Models;

namespace OpencastReplacement.Services
{
    public interface IFfmpegWrapper
    {
        public Task<bool> StartEncoding(Video video, out string message);
        public Task<bool> CancelEncoding(string id);

    }
}
