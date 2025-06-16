using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class SellProductDetailDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Topic { get; set; }
        public string UrlImage { get; set; }
        public string RateName { get; set; } //RateName lấy từ bảng Rarity dựa vào RarityId trong product, lưu ý RarityId trong product là string, rateId trong rarity là objectid
        public string Description { get; set; }//Description lấy trong SellProduct mà repository đã làm
    }
}
