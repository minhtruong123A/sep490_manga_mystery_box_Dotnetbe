using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Permission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("permission_name")]
        public string PermissionName { get; set; }

        [BsonElement("perrmission_code")]
        public string PermissionCode { get; set; }

        [BsonElement("permission_descripition")]
        public string PermissionDescription { get; set; }
    }
}
