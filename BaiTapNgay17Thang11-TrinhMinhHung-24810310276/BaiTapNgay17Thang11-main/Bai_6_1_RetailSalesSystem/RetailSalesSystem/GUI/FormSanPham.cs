using System;
using System.Drawing;
using System.Windows.Forms;
using RetailSalesSystem.BUS;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.GUI
{
    public class FormSanPham : Form
    {
        private readonly SanPhamBUS _bus = new SanPhamBUS();

        private DataGridView dgvSanPham = null!;
        private TextBox txtMaSP = null!;
        private TextBox txtTenSP = null!;
        private NumericUpDown numGiaBan = null!;
        private NumericUpDown numSoLuongTon = null!;
        private TextBox txtTimKiem = null!;
        private Button btnTimKiem = null!;
        private Button btnTatCa = null!;
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        public FormSanPham()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Danh mục Sản phẩm";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(15), BackColor = Color.White };
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ DANH MỤC SẢN PHẨM / HÀNG HÓA",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 10)
            };
            Label lblSub = new Label
            {
                Text = "Tìm kiếm hỗ trợ mã vạch, tên sản phẩm sử dụng SqlParameter (%)",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(17, 40)
            };
            pnlTop.Controls.AddRange(new Control[] { lblTitle, lblSub });

            // Search panel
            Panel pnlSearch = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(15, 5, 15, 5), BackColor = Color.White };
            Label lblSearch = new Label { Text = "Tìm kiếm:", Location = new Point(15, 15), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.Black };
            txtTimKiem = new TextBox { Location = new Point(90, 12), Width = 300, PlaceholderText = "Nhập tên hoặc mã sản phẩm/mã vạch...", BackColor = Color.White, ForeColor = Color.Black };
            btnTimKiem = new Button { Text = "Tìm kiếm", Location = new Point(400, 10), Width = 90, Height = 28, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnTatCa = new Button { Text = "Hiện tất cả", Location = new Point(500, 10), Width = 90, Height = 28, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };

            btnTimKiem.Click += (s, e) => TimKiem();
            btnTatCa.Click += (s, e) => { txtTimKiem.Clear(); LoadData(); };
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) TimKiem(); };

            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtTimKiem, btnTimKiem, btnTatCa });

            // Bottom Input Form
            GroupBox grpInput = new GroupBox
            {
                Text = "Thông tin chi tiết sản phẩm",
                Dock = DockStyle.Bottom,
                Height = 170,
                Padding = new Padding(10),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            Label lblMa = new Label { Text = "Mã SP:", Location = new Point(20, 30), AutoSize = true, ForeColor = Color.Black };
            txtMaSP = new TextBox { Location = new Point(90, 27), Width = 150, BackColor = Color.White, ForeColor = Color.Black };

            Label lblTen = new Label { Text = "Tên SP:", Location = new Point(270, 30), AutoSize = true, ForeColor = Color.Black };
            txtTenSP = new TextBox { Location = new Point(340, 27), Width = 300, BackColor = Color.White, ForeColor = Color.Black };

            Label lblGia = new Label { Text = "Giá bán:", Location = new Point(20, 70), AutoSize = true, ForeColor = Color.Black };
            numGiaBan = new NumericUpDown { Location = new Point(90, 67), Width = 150, Maximum = 1000000000, Increment = 1000, BackColor = Color.White, ForeColor = Color.Black };

            Label lblTon = new Label { Text = "Số lượng tồn:", Location = new Point(270, 70), AutoSize = true, ForeColor = Color.Black };
            numSoLuongTon = new NumericUpDown { Location = new Point(370, 67), Width = 120, Maximum = 100000, BackColor = Color.White, ForeColor = Color.Black };

            btnThem = new Button { Text = "Thêm mới", Location = new Point(20, 115), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), FlatStyle = FlatStyle.System };
            btnSua = new Button { Text = "Cập nhật", Location = new Point(125, 115), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnXoa = new Button { Text = "Xóa", Location = new Point(230, 115), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(335, 115), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };

            btnThem.Click += (s, e) => Them();
            btnSua.Click += (s, e) => Sua();
            btnXoa.Click += (s, e) => Xoa();
            btnLamMoi.Click += (s, e) => ClearForm();

            grpInput.Controls.AddRange(new Control[] {
                lblMa, txtMaSP, lblTen, txtTenSP,
                lblGia, numGiaBan, lblTon, numSoLuongTon,
                btnThem, btnSua, btnXoa, btnLamMoi
            });

            // Grid
            dgvSanPham = new DataGridView
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
            dgvSanPham.DefaultCellStyle.BackColor = Color.White;
            dgvSanPham.DefaultCellStyle.ForeColor = Color.Black;
            dgvSanPham.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 245);
            dgvSanPham.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvSanPham.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvSanPham.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvSanPham.EnableHeadersVisualStyles = false;
            dgvSanPham.CellClick += DgvSanPham_CellClick;

            this.Controls.Add(dgvSanPham);
            this.Controls.Add(grpInput);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var list = _bus.GetAll();
            dgvSanPham.DataSource = null;
            dgvSanPham.DataSource = list;
            FormatGrid();
        }

        private void TimKiem()
        {
            string kw = txtTimKiem.Text.Trim();
            var list = _bus.Search(kw);
            dgvSanPham.DataSource = null;
            dgvSanPham.DataSource = list;
            FormatGrid();
        }

        private void FormatGrid()
        {
            if (dgvSanPham.Columns["MaSP"] != null) dgvSanPham.Columns["MaSP"].HeaderText = "Mã Sản Phẩm";
            if (dgvSanPham.Columns["TenSP"] != null) dgvSanPham.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            if (dgvSanPham.Columns["GiaBan"] != null)
            {
                dgvSanPham.Columns["GiaBan"].HeaderText = "Giá Bán (VNĐ)";
                dgvSanPham.Columns["GiaBan"].DefaultCellStyle.Format = "N0";
            }
            if (dgvSanPham.Columns["SoLuongTon"] != null)
            {
                dgvSanPham.Columns["SoLuongTon"].HeaderText = "Số Lượng Tồn";
                dgvSanPham.Columns["SoLuongTon"].DefaultCellStyle.Format = "N0";
            }
        }

        private void DgvSanPham_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvSanPham.Rows[e.RowIndex].DataBoundItem is SanPhamDTO sp)
            {
                txtMaSP.Text = sp.MaSP;
                txtMaSP.ReadOnly = true;
                txtTenSP.Text = sp.TenSP;
                numGiaBan.Value = sp.GiaBan;
                numSoLuongTon.Value = sp.SoLuongTon;
            }
        }

        private void ClearForm()
        {
            txtMaSP.Clear();
            txtMaSP.ReadOnly = false;
            txtTenSP.Clear();
            numGiaBan.Value = 0;
            numSoLuongTon.Value = 0;
            dgvSanPham.ClearSelection();
        }

        private void Them()
        {
            var sp = new SanPhamDTO
            {
                MaSP = txtMaSP.Text.Trim(),
                TenSP = txtTenSP.Text.Trim(),
                GiaBan = numGiaBan.Value,
                SoLuongTon = (int)numSoLuongTon.Value
            };
            var (success, msg) = _bus.Add(sp);
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Sua()
        {
            var sp = new SanPhamDTO
            {
                MaSP = txtMaSP.Text.Trim(),
                TenSP = txtTenSP.Text.Trim(),
                GiaBan = numGiaBan.Value,
                SoLuongTon = (int)numSoLuongTon.Value
            };
            var (success, msg) = _bus.Update(sp);
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Xoa()
        {
            string maSP = txtMaSP.Text.Trim();
            if (string.IsNullOrEmpty(maSP))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm [{maSP}] không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, msg) = _bus.Delete(maSP);
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
