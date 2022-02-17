using System.Text;

namespace OpencastReplacement.Helpers
{
    public class ComparableList<T> : List<T>
    {
        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }
            var otherList = obj as ComparableList<T> ?? new();
            var diff = this.Except(otherList);
            if(diff?.Count() == 0)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var sb = new StringBuilder();
            foreach(var item in this)
            {
                sb.Append(item?.GetHashCode());
            }
            return sb.GetHashCode();
        }
    }
}
