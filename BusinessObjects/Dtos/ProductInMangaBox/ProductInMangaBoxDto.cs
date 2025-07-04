﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.ProductInMangaBox
{
    public class ProductInMangaBoxDto
    {
        public string MangaBoxId { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Chance { get; set; }
    }
}
