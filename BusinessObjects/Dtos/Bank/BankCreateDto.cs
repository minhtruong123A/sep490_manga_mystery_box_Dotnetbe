using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Bank
{
    public class BankCreateDto
    {
        public string Abbreviation { get; set; }
        public string Name { get; set; }
    }
}
