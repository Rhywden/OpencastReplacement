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
        public record TagSuccess(ImmutableList<Tag> tags);
        public record LoadTags
        {
            public record Request();
            public record Error(string message);
        }
        public record AddTag
        {
            public record Request(Tag tagToBeAdded);
            public record Error(string message);
        }
        public record UpdateTag
        {
            public record Request(Tag tagToBeUpdated);
            public record Error(string message);
        }
        public record DeleteTag
        {
            public record Request(Tag tagToBeDeleted);
            public record Error(string message);
        }
        public record SeriesSuccess(ImmutableList<Series> series);
        public record LoadSeries
        {
            public record Request();
            public record Error(string message);
        }
        public record AddSeries
        {
            public record Request(Series seriesToBeAdded);
            public record Error(string message);
        }
        public record UpdateSeries
        {
            public record Request(Series seriesToBeUpdated);
            public record Error(string message);
        }
        public record DeleteSeries
        {
            public record Request(Series seriesToBeDeleted);
            public record Error(string message);
        }

        public record ConcersionSuccess(ImmutableList<Conversion> conversions);
        public record AddConversion
        {
            public record Request(Conversion conversionToBeAdded);
            public record Error(string message);
        }
        public record UpdateConversion
        {
            public record Request(Conversion conversionToBeUpdated);
            public record Error(string message);
        }
        public record DeleteConversion
        {
            public record Request(Conversion conversionToBeDeleted);
            public record Error(string message);
        }
    }
}
