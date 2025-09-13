namespace BusinessObjects.Dtos.UserCollection;

public class AddCardsToCollectionDto
{
    public string CollectionId { get; set; }
    public List<string> ProductIds { get; set; }
}