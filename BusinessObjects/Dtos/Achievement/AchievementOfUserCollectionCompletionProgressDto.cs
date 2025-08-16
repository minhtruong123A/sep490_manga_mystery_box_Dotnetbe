using BusinessObjects.Dtos.Reward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Achievement
{
    public class AchievementOfUserCollectionCompletionProgressDto
    {
        public string Id { get; set; }
        public string AchievementName { get; set; }
        public string CollectionId { get; set; }
        public string CollectionName { get; set; }
        public List<ReawrdCompletionProgressOfUserCollectionDto> dtos { get; set; }
    }
}
