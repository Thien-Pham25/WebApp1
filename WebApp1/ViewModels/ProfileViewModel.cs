using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebApp1.Models;

namespace WebApp1.ViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }
        public string Specialty { get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime BirthDate { get; set; }

        public string Education { get; set; }
    }
}