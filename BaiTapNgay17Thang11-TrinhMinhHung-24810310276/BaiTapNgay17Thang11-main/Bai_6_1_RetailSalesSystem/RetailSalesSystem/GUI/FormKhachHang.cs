using System;
using System.Drawing;
using System.Windows.Forms;
using RetailSalesSystem.BUS;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.GUI
{
    public class FormKhachHang : Form
    {
        private readonly KhachHangBUS _bus = new KhachHangBUS();

        private DataGridView dgvKhachHang = null!;
        private TextBox txtMaKH = null!;
        private TextBox txtTenKH = null!;
        private TextBox txtDienThoai = null!;
        private NumericUpDown numDiemTichLuy = null!;
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        public FormKhachHang()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Khách hàng & Điểm tích lũy";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15), BackColor = Color.White };
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ DANH MỤC KHÁCH HÀNG & ĐIỂM TÍCH LŨY",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 15)
            };
            pnlTop.Controls.Add(lblTitle);

            // Bottom Input Form
            GroupBox grpInput = new GroupBox
            {
                Text = "Thông tin khách hàng",
                Dock = DockStyle.Bottom,
                Height = 160,
                Padding = new Padding(10),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            Label lblMa = new Label { Text = "Mã KH:", Location = new Point(20, 30), AutoSize = true, ForeColor = Color.Black };
            txtMaKH = new TextBox { Location = new Point(80, 27), Width = 100, ReadOnly = true, PlaceholderText = "(Tự sinh)", BackColor = Color.White, ForeColor = Color.Black };

            Label lblTen = new Label { Text = "Tên KH:", Location = new Point(200, 30), AutoSize = true, ForeColor = Color.Black };
            txtTenKH = new TextBox { Location = new Point(270, 27), Width = 250, BackColor = Color.White, ForeColor = Color.Black };

            Label lblPhone = new Label { Text = "Điện thoại:", Location = new Point(540, 30), AutoSize = true, ForeColor = Color.Black };
            txtDienThoai = new TextBox { Location = new Point(620, 27), Width = 180, BackColor = Color.White, ForeColor = Color.Black };

            Label lblDiem = new Label { Text = "Điểm tích lũy:", Location = new Point(20, 70), AutoSize = true, ForeColor = Color.Black };
            numDiemTichLuy = new NumericUpDown { Location = new Point(120, 67), Width = 100, Maximum = 1000000, BackColor = Color.White, ForeColor = Color.Black };

            btnThem = new Button { Text = "Thêm mới", Location = new Point(20, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnSua = new Button { Text = "Cập nhật", Location = new Point(125, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnXoa = new Button { Text = "Xóa", Location = new Point(230, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(335, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };

            btnThem.Click += (s, e) => Them();
            btnSua.Click += (s, e) => Sua();
            btnXoa.Click += (s, e) => Xoa();
            btnLamMoi.Click += (s, e) => ClearForm();

            grpInput.Controls.AddRange(new Control[] {
                lblMa, txtMaKH, lblTen, txtTenKH,
                lblPhone, txtDienThoai, lblDiem, numDiemTichLuy,
                btnThem, btnSua, btnXoa, btnLamMoi
            });

            // Grid
            dgvKhachHang = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                ForeColor = Color.Black,
                GridColor = Color.LightGray
            };
            dgvKhachHang.DefaultCellStyle.BackColor = Color.White;
            dgvKhachHang.DefaultCellStyle.ForeColor = Color.Black;
            dgvKhachHang.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 245);
            dgvKhachHang.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvKhachHang.EnableHeadersVisualStyles = false;
            dgvKhachHang.CellClick += DgvKhachHang_CellClick;

            this.Controls.Add(dgvKhachHang);
            this.Controls.Add(grpInput);
            this.Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var list = _bus.GetAll();
            dgvKhachHang.DataSource = null;
            dgvKhachHang.DataSource = list;
            FormatGrid();
        }

        private void FormatGrid()
        {
            if (dgvKhachHang.Columns["MaKH"] != null) dgvKhachHang.Columns["MaKH"].HeaderText = "Mã KH";
            if (dgvKhachHang.Columns["TenKH"] != null) dgvKhachHang.Columns["TenKH"].HeaderText = "Họ và Tên Khách Hàng";
            if (dgvKhachHang.Columns["DienThoai"] != null) dgvKhachHang.Columns["DienThoai"].HeaderText = "Số Điện Thoại";
            if (dgvKhachHang.Columns["DiemTichLuy"] != null)
            {
                dgvKhachHang.Columns["DiemTichLuy"].HeaderText = "Điểm Tích Lũy";
                dgvKhachHang.Columns["DiemTichLuy"].DefaultCellStyle.Format = "N0";
            }
        }

        private void DgvKhachHang_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvKhachHang.Rows[e.RowIndex].DataBoundItem is KhachHangDTO kh)
            {
                txtMaKH.Text = kh.MaKH.ToString();
                txtTenKH.Text = kh.TenKH;
                txtDienThoai.Text = kh.DienThoai;
                numDiemTichLuy.Value = kh.DiemTichLuy;
            }
        }

        private void ClearForm()
        {
            txtMaKH.Clear();
            txtTenKH.Clear();
            txtDienThoai.Clear();
            numDiemTichLuy.Value = 0;
            dgvKhachHang.ClearSelection();
        }

        private void Them()
        {
            var kh = new KhachHangDTO
            {
                TenKH = txtTenKH.Text.Trim(),
                DienThoai = txtDienThoai.Text.Trim(),
                DiemTichLuy = (int)numDiemTichLuy.Value
            };
            var (success, msg) = _bus.Add(kh);
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Sua()
        {
            if (!int.TryParse(txtMaKH.Text, out int maKH))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var kh = new KhachHangDTO
            {
                MaKH = maKH,
                TenKH = txtTenKH.Text.Trim(),
                DienThoai = txtDienThoai.Text.Trim(),
                DiemTichLuy = (int)numDiemTichLuy.Value
            };
            var (success, msg) = _bus.Update(kh);
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Xoa()
        {
            if (!int.TryParse(txtMaKH.Text, out int maKH))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa khách hàng #{maKH} không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, msg) = _bus.Delete(maKH);
                MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (success)
                {
                    LoadData();
                    ClearForm();
                }
            }
        }
    }
}
