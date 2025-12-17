using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class LyLich
    {
        public LyLich()
        {
            DiemThis = new HashSet<DiemThi>();
        }

        public string Mshv { get; set; } = null!;
        public string? Tenhv { get; set; }
        public DateTime? Ngaysinh { get; set; }
        public bool? Phai { get; set; }
        public string? Malop { get; set; }

        public virtual Lop? MalopNavigation { get; set; }
        public virtual ICollection<DiemThi> DiemThis { get; set; }
    }
}
