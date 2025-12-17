using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Lop
    {
        public Lop()
        {
            LyLiches = new HashSet<LyLich>();
        }

        public string Malop { get; set; } = null!;
        public string? Tenlop { get; set; }

        public virtual ICollection<LyLich> LyLiches { get; set; }
    }
}
