using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTags.ViewModels
{
    public class LoginInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsRemember { get; set; }
    }
}