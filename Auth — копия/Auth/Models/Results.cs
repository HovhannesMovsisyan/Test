//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Auth.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Results
    {
        public int id { get; set; }
        public Nullable<int> test_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> score { get; set; }
    
        public virtual Test Test { get; set; }
        public virtual User User { get; set; }
    }
}
