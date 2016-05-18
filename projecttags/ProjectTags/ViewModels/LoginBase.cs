using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTags.ViewModels
{
    public class LoginBase
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }

        public int Rank { get; set; }
    }
}