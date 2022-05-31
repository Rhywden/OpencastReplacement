using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections.Immutable;

namespace OpencastReplacement.Helpers
{
    /// <summary>
    /// Represents a serializer for readonly collection.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class ImmutableListSerializer<TItem> :
        EnumerableInterfaceImplementerSerializerBase<System.Collections.Immutable.ImmutableList<TItem>, TItem>
    {
        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableListSerializer{TItem}"/> class.
        /// </summary>
        public ImmutableListSerializer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableListSerializer{TItem}"/> class.
        /// </summary>
        /// <param name="itemSerializer">The item serializer.</param>
        public ImmutableListSerializer(IBsonSerializer<TItem> itemSerializer)
            : base(itemSerializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableListSerializer{TItem}" /> class.
        /// </summary>
        /// <param name="serializerRegistry">The serializer registry.</param>
        public ImmutableListSerializer(IBsonSerializerRegistry serializerRegistry)
            : base(serializerRegistry)
        {
        }

        // public methods
        /// <summary>
        /// Returns a serializer that has been reconfigured with the specified item serializer.
        /// </summary>
        /// <param name="itemSerializer">The item serializer.</param>
        /// <returns>The reconfigured serializer.</returns>
        public ImmutableListSerializer<TItem> WithItemSerializer(IBsonSerializer<TItem> itemSerializer)
        {
            return new ImmutableListSerializer<TItem>(itemSerializer);
        }

        // protected methods
        /// <summary>
        /// Creates the accumulator.
        /// </summary>
        /// <returns>The accumulator.</returns>
        protected override object CreateAccumulator()
        {
            return new List<TItem>();
        }

        /// <summary>
        /// Finalizes the result.
        /// </summary>
        /// <param name="accumulator">The accumulator.</param>
        /// <returns>The final result.</returns>
        protected override System.Collections.Immutable.ImmutableList<TItem> FinalizeResult(object accumulator)
        {
            return ((List<TItem>)accumulator).ToImmutableList();
        }
    }
}
