using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using OpencastReplacement.Models;
using System.Collections.Immutable;

namespace OpencastReplacement.Helpers
{
    public class ImmutableListSerializer : SerializerBase<ImmutableList<Video>>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ImmutableList<Video> value)
        {
            base.Serialize(context, args, value);
        }

        public override ImmutableList<Video> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartArray();

            ImmutableList<Video> Videos = ImmutableList<Video>.Empty;

            while(context.Reader.State != MongoDB.Bson.IO.BsonReaderState.EndOfArray)
            {
                context.Reader.ReadStartDocument();

                var _id = context.Reader.ReadString();
                //TODO: Rest

                context.Reader.ReadEndDocument();
                context.Reader.ReadBsonType();
            }
            context.Reader.ReadEndArray();

            return Videos;

        }
    }
}
