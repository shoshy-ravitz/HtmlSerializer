using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlHelper
    {
        //code for singleton//
        private static readonly HtmlHelper _instance=new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        //code for singleton//


        public List<string> HtmlTags { get; set; }
        public List<string> HtmlVoidTags { get; set; }


        //private -not public becouse singleton
        private HtmlHelper()
        {
            HtmlTags = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("jsonTags/HtmlTags.json"));
           
            HtmlVoidTags = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("jsonTags/HtmlVoidTags.json"));
            
        }

    }
}
