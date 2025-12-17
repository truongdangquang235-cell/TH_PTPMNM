using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class MonHoc
    {
        public MonHoc()
        {
            DiemThis = new HashSet<DiemThi>();
        }

        public string Msmh { get; set; } = null!;
        public string? Tenmh { get; set; }
        public int? Sotiet { get; set; }

        public virtual ICollection<DiemThi> DiemThis { get; set; }
    }
}
