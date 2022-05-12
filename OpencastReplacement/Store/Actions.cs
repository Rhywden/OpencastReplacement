using OpencastReplacement.Models;
using System.Collections.Immutable;

namespace OpencastReplacement.Store
{
    public static class Actions
    {
        public record LoadVideos
        {
            public record Request;
            public record Success(ImmutableList<Video> videos);
            public record Error(string message);
        }
        public record AddVideo
        {
            public record Request(Video videoToBeAdded);
            public record Success(string message);
            public record Error(string message);
        }
        public record UpdateVideo
        {
            public record Request(Video videoToBeUpdated);
            public record Success(ImmutableList<Video> videos);
            public record Error(string message);
        }
        public record DeleteVideo
        {
            public record Request(Video videoToBeDeleted);
            public record Success(ImmutableList<Video> videos);
            public record Error(string message);
        }
    }
}
