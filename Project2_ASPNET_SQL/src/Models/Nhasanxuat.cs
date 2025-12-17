using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Nhasanxuat
    {
        public Nhasanxuat()
        {
            Hanghoas = new HashSet<Hanghoa>();
        }

        public string Mansx { get; set; } = null!;
        public string? Tennsx { get; set; }
        public string? Diachi { get; set; }

        public virtual ICollection<Hanghoa> Hanghoas { get; set; }
    }
}
