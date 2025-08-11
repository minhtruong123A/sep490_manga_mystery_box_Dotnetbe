using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Reward
{
    public class RewardCreateDto
    {
        public int Conditions { get; set; }
        [FromForm]
        public IFormFile? Url_image { get; set; }
        public int Quantity_box { get; set; }
    }
}
