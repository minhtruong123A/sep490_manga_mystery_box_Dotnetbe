using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Options
{
    public class ImageProxySettings
    {
        public List<string> BaseUrls { get; set; } = new();
        public string Path { get; set; } = string.Empty;
    }
}
