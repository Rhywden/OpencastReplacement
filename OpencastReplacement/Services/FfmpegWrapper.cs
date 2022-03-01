

using OpencastReplacement.Models;

namespace OpencastReplacement.Services
{
    public class FfmpegWrapper : IFfmpegWrapper
    {
        private string _pathToFFMPEG;
        private string _storagePath;
        //private Engine _ffmpeg;
        public FfmpegWrapper(string pathToExecutable, string pathToStorageFolder)
        {
            _pathToFFMPEG = pathToExecutable;
            _storagePath = pathToStorageFolder;
            //_ffmpeg = new Engine(pathToExecutable);
        }
        public Task<bool> CancelEncoding(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> StartEncoding(Video video, out string message)
        {
            throw new NotImplementedException();
        }
    }
}
