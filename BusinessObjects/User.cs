using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("username")] public string Username { get; set; }

    [BsonElement("password")] public string Password { get; set; }

    [BsonElement("email")] public string Email { get; set; }

    [BsonElement("profile_image")] public string ProfileImage { get; set; }

    [BsonElement("is_active")] public bool IsActive { get; set; }

    [BsonElement("phone_number")] public string PhoneNumber { get; set; }

    [BsonElement("create_date")] public DateTime CreateDate { get; set; }

    [BsonElement("wallet_id")] public string WalletId { get; set; }

    [BsonElement("wrong_password_count")] public int WrongPasswordCount { get; set; }

    [BsonElement("login_lock_time")] public DateTime? LoginLockTime { get; set; }

    [BsonElement("role_id")] public string RoleId { get; set; }

    [BsonElement("is_email_verification")] public bool EmailVerification { get; set; }
}