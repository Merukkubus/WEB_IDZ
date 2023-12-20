using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Models.Entities;

namespace WebApplication1.Models.ViewModels
{
    public class Basket
    {
        public Guid check_id { get; set; }

        public List<product> prodList { get; set; }

        public Basket(List<product> prList, Guid purID)
        {
            prodList = prList;
            check_id = purID; 
        }
    }
}