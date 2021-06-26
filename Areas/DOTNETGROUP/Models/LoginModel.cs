using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoWeb.Areas.DOTNETGROUP.Models
{
    public class LoginModel
    {
        [Required]
        public string username { get; set; }
        public string password { get; set; }

        public string fullName { get; set; }

        public string email { get; set; }
    }
}