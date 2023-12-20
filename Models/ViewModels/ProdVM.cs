using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class ProdVM
    {
        [Key]
        public long product_id { get; set; }

        [Required]
        [DisplayName("Название")]
        [StringLength(100)]
        public string product_name { get; set; }

        [Required]
        [DisplayName("Категория")]
        public int category_id { get; set; }

        [Required]
        [DisplayName("Цена")]
        public int price1 { get; set; }

        [Required]
        [DisplayName("Производитель")]
        public Guid company_id { get; set; }
    }
}