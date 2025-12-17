using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class DiemThi
    {
        public string Mshv { get; set; } = null!;
        public string Msmh { get; set; } = null!;
        public string? Diem { get; set; }

        public virtual LyLich MshvNavigation { get; set; } = null!;
        public virtual MonHoc MsmhNavigation { get; set; } = null!;
    }
}
