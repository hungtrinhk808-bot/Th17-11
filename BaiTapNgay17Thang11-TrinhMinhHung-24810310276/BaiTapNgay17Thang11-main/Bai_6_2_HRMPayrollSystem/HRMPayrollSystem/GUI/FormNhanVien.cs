using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HRMPayrollSystem.BUS;
using HRMPayrollSystem.DTO;

namespace HRMPayrollSystem.GUI
{
    public class FormNhanVien : Form
    {
        private readonly NhanVienBUS _nhanVienBUS = new NhanVienBUS();
        private readonly PhongBanBUS _phongBanBUS = new PhongBanBUS();

        private DataGridView dgvNhanVien = null!;
        private TextBox txtMaNV = null!;
        private TextBox txtHoTen = null!;
        private DateTimePicker dtpNgaySinh = null!;
        private TextBox txtCCCD = null!;
        private ComboBox cboPhongBan = null!;
        private NumericUpDown numLuongCoBan = null!;
        private NumericUpDown numHeSoLuong = null!;

        // Controls chuyển phòng ban
        private ComboBox cboChuyenPB = null!;
        private Button btnChuyenPB = null!;

        // Search controls
        private TextBox txtTimKiem = null!;
        private Button btnTimKiem = null!;
        private Button btnHienTatCa = null!;

        // CRUD buttons
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        private List<PhongBanDTO> _phongBans = new List<PhongBanDTO>();

        public FormNhanVien()
        {
            InitializeComponent();
            LoadPhongBan();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Hồ sơ Nhân sự";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            // Top title
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(15), BackColor = Color.White };
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ HỒ SƠ NHÂN SỰ (3-TIER ARCHITECTURE)",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 10)
            };
            Label lblSub = new Label
            {
                Text = "Tìm kiếm theo tên, phòng ban, mã CCCD bằng Parameterized Query • Hỗ trợ chuyển phòng ban",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(17, 38)
            };
            pnlTop.Controls.AddRange(new Control[] { lblTitle, lblSub });

            // Search Panel
            Panel pnlSearch = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(15, 5, 15, 5), BackColor = Color.White };
            Label lblSearch = new Label { Text = "Tìm kiếm nhân viên:", Location = new Point(15, 15), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.Black };
            txtTimKiem = new TextBox { Location = new Point(160, 12), Width = 350, PlaceholderText = "Nhập Họ tên, Phòng ban hoặc mã CCCD...", BackColor = Color.White, ForeColor = Color.Black };
            btnTimKiem = new Button { Text = "Tìm kiếm", Location = new Point(520, 10), Width = 95, Height = 28, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnHienTatCa = new Button { Text = "Hiện tất cả", Location = new Point(625, 10), Width = 95, Height = 28, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };

            btnTimKiem.Click += (s, e) => TimKiem();
            btnHienTatCa.Click += (s, e) => { txtTimKiem.Clear(); LoadData(); };
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) TimKiem(); };

            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtTimKiem, btnTimKiem, btnHienTatCa });

            // Bottom Input GroupBox
            GroupBox grpInput = new GroupBox
            {
                Text = "Thông tin chi tiết hồ sơ nhân sự & Điều chuyển phòng ban",
                Dock = DockStyle.Bottom,
                Height = 220,
                Padding = new Padding(10),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            // Row 1
            Label lblMa = new Label { Text = "Mã NV:", Location = new Point(20, 28), AutoSize = true, ForeColor = Color.Black };
            txtMaNV = new TextBox { Location = new Point(90, 25), Width = 90, ReadOnly = true, PlaceholderText = "(Tự sinh)", BackColor = Color.White, ForeColor = Color.Black };

            Label lblTen = new Label { Text = "Họ và tên:", Location = new Point(200, 28), AutoSize = true, ForeColor = Color.Black };
            txtHoTen = new TextBox { Location = new Point(280, 25), Width = 230, BackColor = Color.White, ForeColor = Color.Black };

            Label lblDob = new Label { Text = "Ngày sinh:", Location = new Point(530, 28), AutoSize = true, ForeColor = Color.Black };
            dtpNgaySinh = new DateTimePicker { Location = new Point(610, 25), Width = 150, Format = DateTimePickerFormat.Short };

            Label lblCCCD = new Label { Text = "Số CCCD:", Location = new Point(780, 28), AutoSize = true, ForeColor = Color.Black };
            txtCCCD = new TextBox { Location = new Point(850, 25), Width = 170, BackColor = Color.White, ForeColor = Color.Black };

            // Row 2
            Label lblPB = new Label { Text = "Phòng ban:", Location = new Point(20, 68), AutoSize = true, ForeColor = Color.Black };
            cboPhongBan = new ComboBox { Location = new Point(90, 65), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.White, ForeColor = Color.Black };

            Label lblLuong = new Label { Text = "Lương CB:", Location = new Point(310, 68), AutoSize = true, ForeColor = Color.Black };
            numLuongCoBan = new NumericUpDown { Location = new Point(390, 65), Width = 140, Maximum = 1000000000, Increment = 500000, Value = 10000000, BackColor = Color.White, ForeColor = Color.Black };

            Label lblHeSo = new Label { Text = "Hệ số lương:", Location = new Point(550, 68), AutoSize = true, ForeColor = Color.Black };
            numHeSoLuong = new NumericUpDown { Location = new Point(640, 65), Width = 90, DecimalPlaces = 2, Increment = 0.1m, Maximum = 20, Value = 1.0m, BackColor = Color.White, ForeColor = Color.Black };

            // Row 3: Chuyển phòng ban
            Label lblChuyen = new Label { Text = "Chuyển sang PB:", Location = new Point(750, 68), AutoSize = true, ForeColor = Color.Black, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            cboChuyenPB = new ComboBox { Location = new Point(860, 65), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.White, ForeColor = Color.Black };
            btnChuyenPB = new Button { Text = "Thực hiện Chuyển PB", Location = new Point(860, 100), Width = 160, Height = 32, BackColor = Color.White, ForeColor = Color.Black, Font = new Font("Segoe UI", 9f, FontStyle.Bold), FlatStyle = FlatStyle.System };
            btnChuyenPB.Click += (s, e) => ChuyenPhongBan();

            // Row 4: CRUD buttons
            btnThem = new Button { Text = "Thêm nhân viên", Location = new Point(20, 115), Width = 130, Height = 35, BackColor = Color.White, ForeColor = Color.Black, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), FlatStyle = FlatStyle.System };
            btnSua = new Button { Text = "Cập nhật", Location = new Point(160, 115), Width = 110, Height = 35, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnXoa = new Button { Text = "Xóa", Location = new Point(280, 115), Width = 110, Height = 35, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(400, 115), Width = 110, Height = 35, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };

            btnThem.Click += (s, e) => Them();
            btnSua.Click += (s, e) => Sua();
            btnXoa.Click += (s, e) => Xoa();
            btnLamMoi.Click += (s, e) => ClearForm();

            grpInput.Controls.AddRange(new Control[] {
                lblMa, txtMaNV, lblTen, txtHoTen, lblDob, dtpNgaySinh, lblCCCD, txtCCCD,
                lblPB, cboPhongBan, lblLuong, numLuongCoBan, lblHeSo, numHeSoLuong,
                lblChuyen, cboChuyenPB, btnChuyenPB,
                btnThem, btnSua, btnXoa, btnLamMoi
            });

            // Grid (Nền trắng, chữ đen)
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
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlTop);
        }

        private void LoadPhongBan()
        {
            _phongBans = _phongBanBUS.GetAll();
            cboPhongBan.DataSource = null;
            cboPhongBan.DataSource = new List<PhongBanDTO>(_phongBans);
            cboPhongBan.DisplayMember = "TenPB";
            cboPhongBan.ValueMember = "MaPB";

            cboChuyenPB.DataSource = null;
            cboChuyenPB.DataSource = new List<PhongBanDTO>(_phongBans);
            cboChuyenPB.DisplayMember = "TenPB";
            cboChuyenPB.ValueMember = "MaPB";
        }

        private void LoadData()
        {
            var list = _nhanVienBUS.GetAll();
            dgvNhanVien.DataSource = null;
            dgvNhanVien.DataSource = list;
            FormatGrid();
        }

        private void TimKiem()
        {
            string kw = txtTimKiem.Text.Trim();
            var list = _nhanVienBUS.Search(kw);
            dgvNhanVien.DataSource = null;
            dgvNhanVien.DataSource = list;
            FormatGrid();
        }

        private void FormatGrid()
        {
            if (dgvNhanVien.Columns["MaNV"] != null) dgvNhanVien.Columns["MaNV"].HeaderText = "Mã NV";
            if (dgvNhanVien.Columns["HoTen"] != null) dgvNhanVien.Columns["HoTen"].HeaderText = "Họ và Tên";
            if (dgvNhanVien.Columns["NgaySinh"] != null)
            {
                dgvNhanVien.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
                dgvNhanVien.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            if (dgvNhanVien.Columns["CCCD"] != null) dgvNhanVien.Columns["CCCD"].HeaderText = "Số CCCD";
            if (dgvNhanVien.Columns["TenPB"] != null) dgvNhanVien.Columns["TenPB"].HeaderText = "Phòng Ban";
            if (dgvNhanVien.Columns["LuongCoBan"] != null)
            {
                dgvNhanVien.Columns["LuongCoBan"].HeaderText = "Lương Cơ Bản";
                dgvNhanVien.Columns["LuongCoBan"].DefaultCellStyle.Format = "N0";
            }
            if (dgvNhanVien.Columns["HeSoLuong"] != null)
            {
                dgvNhanVien.Columns["HeSoLuong"].HeaderText = "Hệ Số";
                dgvNhanVien.Columns["HeSoLuong"].DefaultCellStyle.Format = "0.0#";
            }
            if (dgvNhanVien.Columns["MaPB"] != null) dgvNhanVien.Columns["MaPB"].Visible = false;
        }

        private void DgvNhanVien_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvNhanVien.Rows[e.RowIndex].DataBoundItem is NhanVienDTO nv)
            {
                txtMaNV.Text = nv.MaNV.ToString();
                txtHoTen.Text = nv.HoTen;
                if (nv.NgaySinh.HasValue) dtpNgaySinh.Value = nv.NgaySinh.Value;
                txtCCCD.Text = nv.CCCD;
                cboPhongBan.SelectedValue = nv.MaPB;
                numLuongCoBan.Value = nv.LuongCoBan;
                numHeSoLuong.Value = (decimal)nv.HeSoLuong;
            }
        }

        private void ClearForm()
        {
            txtMaNV.Clear();
            txtHoTen.Clear();
            txtCCCD.Clear();
            dtpNgaySinh.Value = DateTime.Today.AddYears(-25);
            numLuongCoBan.Value = 10000000;
            numHeSoLuong.Value = 1.0m;
            if (cboPhongBan.Items.Count > 0) cboPhongBan.SelectedIndex = 0;
            dgvNhanVien.ClearSelection();
        }

        private void Them()
        {
            var nv = new NhanVienDTO
            {
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value,
                CCCD = txtCCCD.Text.Trim(),
                MaPB = cboPhongBan.SelectedValue != null ? (int)cboPhongBan.SelectedValue : 1,
                LuongCoBan = numLuongCoBan.Value,
                HeSoLuong = (double)numHeSoLuong.Value
            };

            var (success, msg) = _nhanVienBUS.Add(nv);
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
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value,
                CCCD = txtCCCD.Text.Trim(),
                MaPB = cboPhongBan.SelectedValue != null ? (int)cboPhongBan.SelectedValue : 1,
                LuongCoBan = numLuongCoBan.Value,
                HeSoLuong = (double)numHeSoLuong.Value
            };

            var (success, msg) = _nhanVienBUS.Update(nv);
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void ChuyenPhongBan()
        {
            if (!int.TryParse(txtMaNV.Text, out int maNV))
            {
                MessageBox.Show("Vui lòng chọn nhân viên muốn chuyển phòng ban!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboChuyenPB.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn phòng ban muốn chuyển đến!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maPBMoi = (int)cboChuyenPB.SelectedValue;
            string tenPBMoi = cboChuyenPB.Text;

            if (MessageBox.Show($"Bạn có chắc muốn chuyển nhân viên [{txtHoTen.Text}] sang phòng [{tenPBMoi}] không?", "Xác nhận chuyển", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, msg) = _nhanVienBUS.ChuyenPhongBan(maNV, maPBMoi);
                MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (success)
                {
                    LoadData();
                }
            }
        }

        private void Xoa()
        {
            if (!int.TryParse(txtMaNV.Text, out int maNV))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa hồ sơ nhân viên #{maNV} không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, msg) = _nhanVienBUS.Delete(maNV);
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
