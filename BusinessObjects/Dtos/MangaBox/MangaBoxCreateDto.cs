using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.MangaBox
{
    public class MangaBoxCreateDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int TotalProduct { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string CollectionTopicId { get; set; }
    }
}
