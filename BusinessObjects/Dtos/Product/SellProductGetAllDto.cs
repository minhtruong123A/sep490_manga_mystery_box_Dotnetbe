using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Product
{
    public class SellProductGetAllDto
    {
        //title product, price, Seller, topic, ==> getall
        //title product, price, Seller, topic, rate, Description
        //=> product, sell_product, user, rarity, user_product
        //exchangecode
        public string Id { get; set; }
        public string Name { get; set; } // get from product
        public int Price { get; set; } // get from sell product
        public string Username { get; set; } // get from user, userid (object) in sell product is Sellerid (string)
        public string Topic { get; set; } // join to  User_Product by Product_Id(object) in Product, Join User_Collection by CollectionId (string) , Join Collection By CollectionId (string) to Get Topic
        public string UrlImage { get; set; } // get from product
    }
}
