using System.Diagnostics.CodeAnalysis;

namespace OpencastReplacement.Helpers
{
    public class StringEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            if(x is null && y is null)
            {
                return true;
            } else if(x is null || y is null)
            {
                return false;
            } else
            {
                return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return obj.GetHashCode();
        }
    }
}
