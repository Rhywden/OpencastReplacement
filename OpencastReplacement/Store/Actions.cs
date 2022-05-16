using OpencastReplacement.Models;
using System.Collections.Immutable;

namespace OpencastReplacement.Store
{
    public static class Actions
    {
        public record VideoSuccess(ImmutableList<Video> videos);
        public record LoadVideos
        {
            public record Request;
            public record Error(string message);
        }
        public record AddVideo
        {
            public record Request(Video videoToBeAdded);
            public record Error(string message);
        }
        public record UpdateVideo
        {
            public record Request(Video videoToBeUpdated);
            public record Error(string message);
        }
        public record DeleteVideo
        {
            public record Request(Video videoToBeDeleted);
            public record Error(string message);
        }
        public record LoadTags
        {
            public record Request();
            public record Success(ImmutableList<Tag> tags);
            public record Error(string message);
        }
        public record AddTag
        {
            public record Request(Tag tagToBeAdded);
            public record Success(ImmutableList<Tag> tags);
            public record Error(string message);
        }
        public record UpdateTag
        {
            public record Request(Tag tagToBeUpdated);
            public record Success(ImmutableList<Tag> tags);
            public record Error(string message);
        }
        public record DeleteTag
        {
            public record Request(Tag tagToBeDeleted);
            public record Success(ImmutableList<Tag> tags);
            public record Error(string message);
        }
        public record LoadSeries
        {
            public record Request();
            public record Success(ImmutableList<Series> series);
            public record Error(string message);
        }
        public record AddSeries
        {
            public record Request(Series seriesToBeAdded);
            public record Success(ImmutableList<Series> series);
            public record Error(string message);
        }
        public record UpdateSeries
        {
            public record Request(Series seriesToBeUpdated);
            public record Success(ImmutableList<Series> series);
            public record Error(string message);
        }
        public record DeleteSeries
        {
            public record Request(Series seriesToBeDeleted);
            public record Success(ImmutableList<Series> series);
            public record Error(string message);
        }
    }
}
