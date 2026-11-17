using System;
using System.Drawing;
using System.Windows.Forms;
using RetailSalesSystem.BUS;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.GUI
{
    public class FormBaoCao : Form
    {
        private readonly BaoCaoBUS _bus = new BaoCaoBUS();

        private Label lblDoanhThuNgay = null!;
        private Label lblDoanhThuThang = null!;
        private DataGridView dgvTop5 = null!;
        private Button btnLamMoi = null!;

        public FormBaoCao()
        {
            InitializeComponent();
            LoadReport();
        }

        private void InitializeComponent()
        {
            this.Text = "Báo cáo - Thống kê Doanh thu & Sản phẩm Bán chạy";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(15), BackColor = Color.White };
            Label lblTitle = new Label
            {
                Text = "BÁO CÁO DOANH THU & TOP SẢN PHẨM BÁN CHẠY NHẤT",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 15)
            };
            btnLamMoi = new Button
            {
                Text = "Làm mới dữ liệu",
                Location = new Point(780, 15),
                Width = 130,
                Height = 35,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                FlatStyle = FlatStyle.System
            };
            btnLamMoi.Click += (s, e) => LoadReport();
            pnlTop.Controls.AddRange(new Control[] { lblTitle, btnLamMoi });

            // Stat Cards Panel (Nền trắng, viền xám, chữ đen)
            Panel pnlCards = new Panel { Dock = DockStyle.Top, Height = 140, Padding = new Padding(15), BackColor = Color.White };

            // Card 1: Doanh thu trong ngay
            Panel card1 = new Panel
            {
                Location = new Point(30, 15),
                Size = new Size(400, 105),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Label lblCard1Title = new Label
            {
                Text = "DOANH THU HÔM NAY (ExecuteScalar)",
                Location = new Point(15, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.Black
            };
            lblDoanhThuNgay = new Label
            {
                Text = "0 VNĐ",
                Location = new Point(15, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.Black
            };
            card1.Controls.AddRange(new Control[] { lblCard1Title, lblDoanhThuNgay });

            // Card 2: Doanh thu trong thang
            Panel card2 = new Panel
            {
                Location = new Point(480, 15),
                Size = new Size(400, 105),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Label lblCard2Title = new Label
            {
                Text = $"DOANH THU THÁNG {DateTime.Today.Month}/{DateTime.Today.Year} (ExecuteScalar)",
                Location = new Point(15, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.Black
            };
            lblDoanhThuThang = new Label
            {
                Text = "0 VNĐ",
                Location = new Point(15, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.Black
            };
            card2.Controls.AddRange(new Control[] { lblCard2Title, lblDoanhThuThang });

            pnlCards.Controls.AddRange(new Control[] { card1, card2 });

            // Grid Box for Top 5
            GroupBox grpTop5 = new GroupBox
            {
                Text = "DANH SÁCH TOP 5 SẢN PHẨM BÁN CHẠY NHẤT (Đọc qua SqlDataReader)",
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            dgvTop5 = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                GridColor = Color.LightGray
            };
            dgvTop5.DefaultCellStyle.BackColor = Color.White;
            dgvTop5.DefaultCellStyle.ForeColor = Color.Black;
            dgvTop5.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 245);
            dgvTop5.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvTop5.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvTop5.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvTop5.EnableHeadersVisualStyles = false;
            grpTop5.Controls.Add(dgvTop5);

            this.Controls.Add(grpTop5);
            this.Controls.Add(pnlCards);
            this.Controls.Add(pnlTop);
        }

        private void LoadReport()
        {
            decimal dtNgay = _bus.GetDoanhThuHomNay();
            decimal dtThang = _bus.GetDoanhThuThangNay();

            lblDoanhThuNgay.Text = $"{dtNgay:N0} VNĐ";
            lblDoanhThuThang.Text = $"{dtThang:N0} VNĐ";

            var list = _bus.GetTop5SanPham();
            dgvTop5.DataSource = null;
            dgvTop5.DataSource = list;

            if (dgvTop5.Columns["MaSP"] != null) dgvTop5.Columns["MaSP"].HeaderText = "Mã Sản Phẩm";
            if (dgvTop5.Columns["TenSP"] != null) dgvTop5.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            if (dgvTop5.Columns["TongSoLuongBan"] != null)
            {
                dgvTop5.Columns["TongSoLuongBan"].HeaderText = "Tổng Số Lượng Đã Bán";
                dgvTop5.Columns["TongSoLuongBan"].DefaultCellStyle.Format = "N0";
            }
            if (dgvTop5.Columns["TongDoanhThu"] != null)
            {
                dgvTop5.Columns["TongDoanhThu"].HeaderText = "Tổng Doanh Thu Mang Về (VNĐ)";
                dgvTop5.Columns["TongDoanhThu"].DefaultCellStyle.Format = "N0";
            }
        }
    }
}
