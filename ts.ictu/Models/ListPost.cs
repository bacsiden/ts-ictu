using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace ts.ictu.Models
{
    public class ListPost
    {
        public string Title { get; set; }
        public string BottomTitle { get; set; }
        public List<Post> HotPost { get; set; }
        public List<Post> RelatePost { get; set; }
    }
}