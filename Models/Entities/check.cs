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
    
    public partial class check
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public check()
        {
            this.product_in_check = new HashSet<product_in_check>();
        }
    
        public System.Guid check_id { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
        public long card_id { get; set; }
        public System.Guid cashier_id { get; set; }
        public int cash_reg_id { get; set; }
    
        public virtual cash_reg cash_reg { get; set; }
        public virtual cashier cashier { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<product_in_check> product_in_check { get; set; }
        public virtual discount_card discount_card { get; set; }
    }
}
