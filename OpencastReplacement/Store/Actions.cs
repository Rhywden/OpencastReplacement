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
    }
}
