namespace OpencastReplacement.Services
{
    public interface IFfmpegWrapper
    {
        public Task<bool> StartEncoding();
        public Task<bool> CancelEncoding(string id);

    }
}
