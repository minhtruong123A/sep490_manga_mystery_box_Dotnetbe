using BusinessObjects.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.UserCollection
{
    public class UserCollectionGetAllDto
    {
        public string UserId { get; set; }
        public string CollectionId { get; set; }
        public string CollectionTopic { get; set; }
        public List<Collection_sProductsImageDto> Image {  get; set; }
        public int Count { get; set; }
    }
}
