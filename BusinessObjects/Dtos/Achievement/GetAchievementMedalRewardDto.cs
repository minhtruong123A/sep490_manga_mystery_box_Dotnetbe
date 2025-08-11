using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Achievement
{
    public class GetAchievementMedalRewardDto
    {
        public string userRewardId {  get; set; }
        public bool isPublic { get; set; }
        public string UrlImage { get; set; }

    }
}
