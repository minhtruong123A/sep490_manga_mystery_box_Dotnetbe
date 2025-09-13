namespace BusinessObjects.Dtos.Verification;

public class VerificationResultDto
{
    public bool IsVerified { get; set; }
    public string FoundId { get; set; }
    public string ObjectType { get; set; }
    public object FoundObject { get; set; }
}