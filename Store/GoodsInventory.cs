//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class GoodsInventory
    {
        public int Id { get; set; }
        public int GoodsId { get; set; }
        public int InventoryId { get; set; }
    
        public virtual Goods Goods { get; set; }
        public virtual Inventory Inventory { get; set; }
    }
}
