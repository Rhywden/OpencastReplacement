using OpencastReplacement.Models;

namespace OpencastReplacement.Helpers
{
    public class MongoEntryComparer : IEqualityComparer<IMongoEntry>
    {
        public bool Equals(IMongoEntry? entry1, IMongoEntry? entry2)
        {
            if(entry1 is null && entry2 is null)
            {
                return true;
            } else if(entry1 is null || entry2 is null)
            {
                return false;
            } else if(entry1.Id.Equals(entry2.Id))
            {
                return true;
            }
            return false;
        }
        public int GetHashCode(IMongoEntry entry)
        {
            return entry.Id.GetHashCode();
        }
    }
}
