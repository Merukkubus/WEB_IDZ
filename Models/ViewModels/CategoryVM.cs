
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class CategoryVM
    {
        [Key]
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [DisplayName("Название")]
        [StringLength(70)]
        public string CategoryName { get; set; }
    }
}