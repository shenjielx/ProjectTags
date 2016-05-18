using ProjectTags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTags.ViewModels
{
    public class HomeModels: PagerModel
    {
        public Dictionary<string,int> DictCount { get; set; }
        public IEnumerable<Tasks> Tasks { get; set; }
        public IEnumerable<Processes> Processes { get; set; }
    }
}