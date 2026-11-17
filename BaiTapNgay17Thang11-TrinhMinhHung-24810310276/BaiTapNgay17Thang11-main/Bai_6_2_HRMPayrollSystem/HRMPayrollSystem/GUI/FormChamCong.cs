using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using HRMPayrollSystem.BUS;
using HRMPayrollSystem.DTO;

namespace HRMPayrollSystem.GUI
{
    public class FormChamCong : Form
    {
        private readonly ChamCongBUS _bus = new ChamCongBUS();
        private readonly NhanVienBUS _nhanVienBUS = new NhanVienBUS();

        private ComboBox cboThang = null!;
        private NumericUpDown numNam = null!;
        private Button btnNapDuLieu = null!;
        private Button btnLuuChamCong = null!;
        private Label lblTrangThai = null!;
        private DataGridView dgvChamCong = null!;

        // Disconnected model objects
        private DataTable? _dtChamCong = null;
        private SqlDataAdapter? _adapter = null;
        private SqlConnection? _conn = null;
        private Dictionary<int, string> _dictTenNV = new Dictionary<int, string>();

        public FormChamCong()
        {
            InitializeComponent();
            LoadNhanVienLookup();
            NapDuLieu();
        }

        private void InitializeComponent()
        {
            this.Text = "Chấm công nhân viên hàng tháng (Disconnected Model)";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(15), BackColor = Color.White };
            
            Label lblTitle = new Label
            {
                Text = "BẢNG CHẤM CÔNG HÀNG THÁNG (DISCONNECTED MODEL)",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 10)
            };

            Label lblThang = new Label { Text = "Chọn Tháng:", Location = new Point(18, 55), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.Black };
            cboThang = new ComboBox { Location = new Point(105, 52), Width = 70, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.White, ForeColor = Color.Black };
            for (int i = 1; i <= 12; i++) cboThang.Items.Add(i.ToString("D2"));
            cboThang.SelectedItem = DateTime.Today.Month.ToString("D2");

            Label lblNam = new Label { Text = "Năm:", Location = new Point(190, 55), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.Black };
            numNam = new NumericUpDown { Location = new Point(235, 52), Width = 80, Minimum = 2020, Maximum = 2050, Value = DateTime.Today.Year, BackColor = Color.White, ForeColor = Color.Black };

            btnNapDuLieu = new Button
            {
                Text = "Nạp Bảng Chấm Công",
                Location = new Point(330, 48),
                Width = 160,
                Height = 32,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.System
            };
            btnNapDuLieu.Click += (s, e) => NapDuLieu();

            btnLuuChamCong = new Button
            {
                Text = "💾  LƯU CHẤM CÔNG (SqlCommandBuilder)",
                Location = new Point(510, 48),
                Width = 260,
                Height = 32,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.System
            };
            btnLuuChamCong.Click += (s, e) => LuuChamCong();

            lblTrangThai = new Label
            {
                Text = "",
                Location = new Point(780, 55),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.Black
            };

            pnlTop.Controls.AddRange(new Control[] {
                lblTitle, lblThang, cboThang, lblNam, numNam,
                btnNapDuLieu, btnLuuChamCong, lblTrangThai
            });

            // Grid (Nền trắng, chữ đen)
            dgvChamCong = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                ForeColor = Color.Black,
                GridColor = Color.LightGray
            };
            dgvChamCong.DefaultCellStyle.BackColor = Color.White;
            dgvChamCong.DefaultCellStyle.ForeColor = Color.Black;
            dgvChamCong.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 245);
            dgvChamCong.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvChamCong.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvChamCong.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvChamCong.EnableHeadersVisualStyles = false;

            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 40, Padding = new Padding(15, 8, 15, 8), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            Label lblNote = new Label
            {
                Text = "💡 Hướng dẫn: Người dùng nhập trực tiếp 'Số ngày công' (<= 31) và 'Số giờ OT' trên lưới, sau đó ấn 'Lưu chấm công' để đồng bộ về CSDL trong 1 thao tác.",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.Black
            };
            pnlBottom.Controls.Add(lblNote);

            this.Controls.Add(dgvChamCong);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(pnlTop);
        }

        private void LoadNhanVienLookup()
        {
            var nvs = _nhanVienBUS.GetAll();
            _dictTenNV.Clear();
            foreach (var nv in nvs)
            {
                _dictTenNV[nv.MaNV] = $"{nv.HoTen} ({nv.TenPB})";
            }
        }

        private string GetThangNam()
        {
            return $"{cboThang.SelectedItem}/{numNam.Value}";
        }

        private void NapDuLieu()
        {
            string thangNam = GetThangNam();
            LoadNhanVienLookup();

            _conn?.Dispose();

            var (dt, adapter, conn) = _bus.GetChamCong(thangNam);
            _dtChamCong = dt;
            _adapter = adapter;
            _conn = conn;

            if (!_dtChamCong.Columns.Contains("HoTenNhanVien"))
            {
                DataColumn colHoTen = new DataColumn("HoTenNhanVien", typeof(string));
                _dtChamCong.Columns.Add(colHoTen);
            }

            foreach (DataRow row in _dtChamCong.Rows)
            {
                int maNV = Convert.ToInt32(row["MaNV"]);
                if (_dictTenNV.ContainsKey(maNV))
                {
                    row["HoTenNhanVien"] = _dictTenNV[maNV];
                }
            }
            _dtChamCong.AcceptChanges();

            dgvChamCong.DataSource = null;
            dgvChamCong.DataSource = _dtChamCong;

            FormatGrid();

            bool isLocked = _bus.IsMonthLocked(thangNam);
            if (isLocked)
            {
                lblTrangThai.Text = "🔒 ĐÃ CHỐT LƯƠNG (BỊ KHÓA)";
                lblTrangThai.ForeColor = Color.Black;
                btnLuuChamCong.Enabled = false;
                dgvChamCong.ReadOnly = true;
            }
            else
            {
                lblTrangThai.Text = "🔓 Đang mở chỉnh sửa";
                lblTrangThai.ForeColor = Color.Black;
                btnLuuChamCong.Enabled = true;
                dgvChamCong.ReadOnly = false;

                if (dgvChamCong.Columns["MaCong"] != null) dgvChamCong.Columns["MaCong"].ReadOnly = true;
                if (dgvChamCong.Columns["MaNV"] != null) dgvChamCong.Columns["MaNV"].ReadOnly = true;
                if (dgvChamCong.Columns["HoTenNhanVien"] != null) dgvChamCong.Columns["HoTenNhanVien"].ReadOnly = true;
                if (dgvChamCong.Columns["ThangNam"] != null) dgvChamCong.Columns["ThangNam"].ReadOnly = true;
                if (dgvChamCong.Columns["DaKhoa"] != null) dgvChamCong.Columns["DaKhoa"].ReadOnly = true;

                if (dgvChamCong.Columns["SoNgayCong"] != null) dgvChamCong.Columns["SoNgayCong"].ReadOnly = false;
                if (dgvChamCong.Columns["SoGioOT"] != null) dgvChamCong.Columns["SoGioOT"].ReadOnly = false;
            }
        }

        private void FormatGrid()
        {
            if (dgvChamCong.Columns["MaCong"] != null) dgvChamCong.Columns["MaCong"].HeaderText = "Mã Công";
            if (dgvChamCong.Columns["MaNV"] != null) dgvChamCong.Columns["MaNV"].HeaderText = "Mã NV";
            if (dgvChamCong.Columns["HoTenNhanVien"] != null)
            {
                dgvChamCong.Columns["HoTenNhanVien"].HeaderText = "Họ Tên & Phòng Ban";
                dgvChamCong.Columns["HoTenNhanVien"].DisplayIndex = 2;
            }
            if (dgvChamCong.Columns["ThangNam"] != null) dgvChamCong.Columns["ThangNam"].HeaderText = "Tháng/Năm";
            if (dgvChamCong.Columns["SoNgayCong"] != null)
            {
                dgvChamCong.Columns["SoNgayCong"].HeaderText = "Số Ngày Công (Sửa)";
                dgvChamCong.Columns["SoNgayCong"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
            if (dgvChamCong.Columns["SoGioOT"] != null)
            {
                dgvChamCong.Columns["SoGioOT"].HeaderText = "Số Giờ Làm Thêm (OT) (Sửa)";
                dgvChamCong.Columns["SoGioOT"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
            if (dgvChamCong.Columns["DaKhoa"] != null) dgvChamCong.Columns["DaKhoa"].HeaderText = "Đã Khóa?";
        }

        private void LuuChamCong()
        {
            if (_dtChamCong == null || _adapter == null || _conn == null)
            {
                MessageBox.Show("Chưa có dữ liệu để lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvChamCong.EndEdit();

            var (success, msg) = _bus.SaveChamCong(_dtChamCong, _adapter, _conn);
            MessageBox.Show(
                msg,
                success ? "Đồng bộ thành công" : "Lỗi lưu dữ liệu",
                MessageBoxButtons.OK,
                success ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (success)
            {
                NapDuLieu();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _conn?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
