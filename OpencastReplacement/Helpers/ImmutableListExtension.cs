using OpencastReplacement.Models;
using System.Collections.Immutable;

namespace OpencastReplacement.Helpers
{
    public static class ImmutableListExtension
    {
        public static ImmutableList<IMongoEntry> Replace(this ImmutableList<IMongoEntry> list, IMongoEntry entry)
        {
            int index = list.FindIndex(li => li.Id.Equals(entry.Id));
            if (index == -1)
            {
                return list;
            }
            return list.SetItem(index, entry);
        }
    }
}
