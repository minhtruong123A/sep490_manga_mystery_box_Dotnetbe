using BusinessObjects.Dtos.Reward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Achievement
{
    public class AchievementWithRewardsCreateDto
    {
        public string Name_Achievement { get; set; }
        public List<RewardCreateDto> dtos {  get; set; }
    }
}
