using BusinessObjects.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.MangaBox
{
    public class MangaBoxDetailDto
    {
        public string Id { get; set; }
        public int Status { get; set; }
        public string MysteryBoxName { get; set; }
        public string MysteryBoxDescription { get; set; }
        public int MysteryBoxPrice { get; set; }
        public string CollectionTopic { get; set; }
        public string UrlImage { get; set; }
        public int TotalProduct { get; set; }
        public List<ProductInBoxDto> Products { get; set; }
    }
}
