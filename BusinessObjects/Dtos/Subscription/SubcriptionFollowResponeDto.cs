using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Subscription
{
    public class SubcriptionFollowResponeDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FollowerId { get; set; }
        public string UrlImage { get; set; }
        public DateTime Follow_at { get; set; }
    }
}
