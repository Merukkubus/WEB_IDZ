using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class EditUserVM
    {
        [Key]
        public System.Guid Id {  get; set; }
        public string login { get; set; }
        [Required]
        public long card_id { get; set; }

        [Required]
        public int role_id { get; set; }

        public string passwordhash { get; set; }

        public System.Guid salt { get; set; }
    }
}