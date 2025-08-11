using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Reward
{
    public class RewardGetDto
    {
        public int Conditions { get; set; }
        public string? Url_image { get; set; }
        public string MangaBoxId { get; set; }
        public int Quantity_box { get; set; }
    }
}
