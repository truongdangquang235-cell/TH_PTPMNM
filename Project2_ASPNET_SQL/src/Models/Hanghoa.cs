using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Hanghoa
    {
        public Hanghoa()
        {
            Chitietphieudathangs = new HashSet<Chitietphieudathang>();
            Chitietphieugiaohangs = new HashSet<Chitietphieugiaohang>();
        }

        public string Mahang { get; set; } = null!;
        public string? Tenhang { get; set; }
        public string? Donvitinh { get; set; }
        public double? Dongia { get; set; }
        public string? Hinh { get; set; }
        public string? Maloai { get; set; }
        public string? Mansx { get; set; }

        public virtual Loaihanghoa? MaloaiNavigation { get; set; }
        public virtual Nhasanxuat? MansxNavigation { get; set; }
        public virtual ICollection<Chitietphieudathang> Chitietphieudathangs { get; set; }
        public virtual ICollection<Chitietphieugiaohang> Chitietphieugiaohangs { get; set; }
    }
}
