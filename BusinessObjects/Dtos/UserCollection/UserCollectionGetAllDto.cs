using BusinessObjects.Dtos.Product;

namespace BusinessObjects.Dtos.UserCollection;

public class UserCollectionGetAllDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string CollectionId { get; set; }
    public string CollectionTopic { get; set; }
    public List<Collection_sProductsImageDto> Image { get; set; }
    public int Count { get; set; }
}