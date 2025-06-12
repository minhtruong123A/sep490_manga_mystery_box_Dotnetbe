using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Serializers
{
    public class FlexibleSerializer<T> : SerializerBase<T>
    {
        private readonly Func<BsonDeserializationContext, BsonDeserializationArgs, T> _customDeserializer;
        private readonly Action<BsonSerializationContext, BsonSerializationArgs, T> _customSerializer;

        public FlexibleSerializer(
            Func<BsonDeserializationContext, BsonDeserializationArgs, T> customDeserializer,
            Action<BsonSerializationContext, BsonSerializationArgs, T> customSerializer)
        {
            _customDeserializer = customDeserializer ?? throw new ArgumentNullException(nameof(customDeserializer));
            _customSerializer = customSerializer ?? throw new ArgumentNullException(nameof(customSerializer));
        }

        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => _customDeserializer(context, args);
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value) => _customSerializer(context, args, value);

    }
}
