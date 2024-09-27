using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlogAPI.Class
{
    public class APIRequest
    {
        public string Action { get; set; }
        public Class.Post? Post {  get; set; }
    }
}
