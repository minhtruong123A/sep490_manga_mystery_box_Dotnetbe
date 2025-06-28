using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.UserCollection
{
    public class AddCardsToCollectionDto
    {
        public string CollectionId { get; set; }
        public List<string> ProductIds { get; set; }
    }

}
