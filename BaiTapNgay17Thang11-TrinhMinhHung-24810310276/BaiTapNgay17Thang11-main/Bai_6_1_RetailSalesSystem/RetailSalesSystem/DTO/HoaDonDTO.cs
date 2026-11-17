using System;

namespace RetailSalesSystem.DTO
{
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public int MaNV { get; set; }
        public string TenNV { get; set; } = string.Empty;
        public int? MaKH { get; set; }
        public string TenKH { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
    }
}
