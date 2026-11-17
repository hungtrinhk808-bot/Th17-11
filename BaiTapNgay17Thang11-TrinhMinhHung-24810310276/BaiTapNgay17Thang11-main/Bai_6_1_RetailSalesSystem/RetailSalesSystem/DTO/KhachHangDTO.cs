namespace RetailSalesSystem.DTO
{
    public class KhachHangDTO
    {
        public int MaKH { get; set; }
        public string TenKH { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public int DiemTichLuy { get; set; } = 0;

        public override string ToString()
        {
            return $"{TenKH} - {DienThoai} (Điểm: {DiemTichLuy})";
        }
    }
}
