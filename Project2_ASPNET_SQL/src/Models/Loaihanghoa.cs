using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Loaihanghoa
    {
        public Loaihanghoa()
        {
            Hanghoas = new HashSet<Hanghoa>();
        }

        public string Maloai { get; set; } = null!;
        public string? Tenloai { get; set; }

        public virtual ICollection<Hanghoa> Hanghoas { get; set; }
    }
}
