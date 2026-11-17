using System;
using System.Drawing;
using System.Windows.Forms;
using RetailSalesSystem.BUS;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.GUI
{
    public class FormNhanVien : Form
    {
        private readonly NhanVienBUS _bus = new NhanVienBUS();

        private DataGridView dgvNhanVien = null!;
        private TextBox txtMaNV = null!;
        private TextBox txtTenNV = null!;
        private TextBox txtSoDienThoai = null!;
        private ComboBox cboChucVu = null!;
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        public FormNhanVien()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Nhân viên bán hàng";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15), BackColor = Color.White };
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ DANH MỤC NHÂN VIÊN BÁN HÀNG",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 15)
            };
            pnlTop.Controls.Add(lblTitle);

            // Bottom Input Form
            GroupBox grpInput = new GroupBox
            {
                Text = "Thông tin nhân viên",
                Dock = DockStyle.Bottom,
                Height = 160,
                Padding = new Padding(10),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            Label lblMa = new Label { Text = "Mã NV:", Location = new Point(20, 30), AutoSize = true, ForeColor = Color.Black };
            txtMaNV = new TextBox { Location = new Point(80, 27), Width = 100, ReadOnly = true, PlaceholderText = "(Tự sinh)", BackColor = Color.White, ForeColor = Color.Black };

            Label lblTen = new Label { Text = "Tên NV:", Location = new Point(200, 30), AutoSize = true, ForeColor = Color.Black };
            txtTenNV = new TextBox { Location = new Point(270, 27), Width = 230, BackColor = Color.White, ForeColor = Color.Black };

            Label lblPhone = new Label { Text = "Số ĐT:", Location = new Point(520, 30), AutoSize = true, ForeColor = Color.Black };
            txtSoDienThoai = new TextBox { Location = new Point(580, 27), Width = 180, BackColor = Color.White, ForeColor = Color.Black };

            Label lblChucVu = new Label { Text = "Chức vụ:", Location = new Point(20, 70), AutoSize = true, ForeColor = Color.Black };
            cboChucVu = new ComboBox { Location = new Point(80, 67), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.White, ForeColor = Color.Black };
            cboChucVu.Items.AddRange(new object[] { "Thu ngân", "Quản lý", "Nhân viên bán hàng", "Kiểm kho" });
            cboChucVu.SelectedIndex = 0;

            btnThem = new Button { Text = "Thêm mới", Location = new Point(20, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnSua = new Button { Text = "Cập nhật", Location = new Point(125, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnXoa = new Button { Text = "Xóa", Location = new Point(230, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(335, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };

            btnThem.Click += (s, e) => Them();
            btnSua.Click += (s, e) => Sua();
            btnXoa.Click += (s, e) => Xoa();
            btnLamMoi.Click += (s, e) => ClearForm();

            grpInput.Controls.AddRange(new Control[] {
                lblMa, txtMaNV, lblTen, txtTenNV,
                lblPhone, txtSoDienThoai, lblChucVu, cboChucVu,
                btnThem, btnSua, btnXoa, btnLamMoi
            });

            // Grid
            dgvNhanVien = new DataGridView
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
            dgvNhanVien.DefaultCellStyle.BackColor = Color.White;
            dgvNhanVien.DefaultCellStyle.ForeColor = Color.Black;
            dgvNhanVien.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 245);
            dgvNhanVien.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvNhanVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvNhanVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvNhanVien.EnableHeadersVisualStyles = false;
            dgvNhanVien.CellClick += DgvNhanVien_CellClick;

            this.Controls.Add(dgvNhanVien);
            this.Controls.Add(grpInput);
            this.Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var list = _bus.GetAll();
            dgvNhanVien.DataSource = null;
            dgvNhanVien.DataSource = list;
            FormatGrid();
        }

        private void FormatGrid()
        {
            if (dgvNhanVien.Columns["MaNV"] != null) dgvNhanVien.Columns["MaNV"].HeaderText = "Mã NV";
            if (dgvNhanVien.Columns["TenNV"] != null) dgvNhanVien.Columns["TenNV"].HeaderText = "Họ và Tên Nhân Viên";
            if (dgvNhanVien.Columns["SoDienThoai"] != null) dgvNhanVien.Columns["SoDienThoai"].HeaderText = "Số Điện Thoại";
            if (dgvNhanVien.Columns["ChucVu"] != null) dgvNhanVien.Columns["ChucVu"].HeaderText = "Chức Vụ";
        }

        private void DgvNhanVien_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvNhanVien.Rows[e.RowIndex].DataBoundItem is NhanVienDTO nv)
            {
                txtMaNV.Text = nv.MaNV.ToString();
                txtTenNV.Text = nv.TenNV;
                txtSoDienThoai.Text = nv.SoDienThoai;
                int idx = cboChucVu.Items.IndexOf(nv.ChucVu);
                if (idx >= 0) cboChucVu.SelectedIndex = idx;
                else cboChucVu.Text = nv.ChucVu;
            }
        }

        private void ClearForm()
        {
            txtMaNV.Clear();
            txtTenNV.Clear();
            txtSoDienThoai.Clear();
            cboChucVu.SelectedIndex = 0;
            dgvNhanVien.ClearSelection();
        }

        private void Them()
        {
            var nv = new NhanVienDTO
            {
                TenNV = txtTenNV.Text.Trim(),
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                ChucVu = cboChucVu.Text
            };
            var (success, msg) = _bus.Add(nv);
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Sua()
        {
            if (!int.TryParse(txtMaNV.Text, out int maNV))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nv = new NhanVienDTO
            {
                MaNV = maNV,
                TenNV = txtTenNV.Text.Trim(),
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                ChucVu = cboChucVu.Text
            };
            var (success, msg) = _bus.Update(nv);
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Xoa()
        {
            if (!int.TryParse(txtMaNV.Text, out int maNV))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa nhân viên #{maNV} không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, msg) = _bus.Delete(maNV);
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
