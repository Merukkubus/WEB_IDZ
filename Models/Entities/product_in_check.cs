//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class product_in_check
    {
        public long product_id { get; set; }
        public System.Guid check_id { get; set; }
        public int quantity { get; set; }
    
        public virtual check check { get; set; }
        public virtual product product { get; set; }
    }
}