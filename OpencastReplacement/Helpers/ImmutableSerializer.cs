using MongoDB.Bson.Serialization;

namespace OpencastReplacement.Helpers
{
    public class ImmutableSerializer : IBsonSerializer
    {
        public Type ValueType => throw new NotImplementedException();

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            throw new NotImplementedException();
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            throw new NotImplementedException();
        }
    }
}
