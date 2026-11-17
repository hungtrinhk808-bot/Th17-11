using System;
using System.Drawing;
using System.Windows.Forms;
using HRMPayrollSystem.BUS;
using HRMPayrollSystem.DTO;

namespace HRMPayrollSystem.GUI
{
    public class FormPhongBan : Form
    {
        private readonly PhongBanBUS _bus = new PhongBanBUS();

        private DataGridView dgvPhongBan = null!;
        private TextBox txtMaPB = null!;
        private TextBox txtTenPB = null!;
        private TextBox txtMoTa = null!;
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        public FormPhongBan()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Phòng ban";
            this.Size = new Size(850, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15), BackColor = Color.White };
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ DANH MỤC PHÒNG BAN",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 15)
            };
            pnlTop.Controls.Add(lblTitle);

            // Bottom Input Form
            GroupBox grpInput = new GroupBox
            {
                Text = "Thông tin phòng ban",
                Dock = DockStyle.Bottom,
                Height = 160,
                Padding = new Padding(10),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            Label lblMa = new Label { Text = "Mã PB:", Location = new Point(20, 30), AutoSize = true, ForeColor = Color.Black };
            txtMaPB = new TextBox { Location = new Point(80, 27), Width = 100, ReadOnly = true, PlaceholderText = "(Tự sinh)", BackColor = Color.White, ForeColor = Color.Black };

            Label lblTen = new Label { Text = "Tên PB:", Location = new Point(200, 30), AutoSize = true, ForeColor = Color.Black };
            txtTenPB = new TextBox { Location = new Point(260, 27), Width = 260, BackColor = Color.White, ForeColor = Color.Black };

            Label lblMoTa = new Label { Text = "Mô tả:", Location = new Point(20, 70), AutoSize = true, ForeColor = Color.Black };
            txtMoTa = new TextBox { Location = new Point(80, 67), Width = 600, BackColor = Color.White, ForeColor = Color.Black };

            btnThem = new Button { Text = "Thêm mới", Location = new Point(20, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnSua = new Button { Text = "Cập nhật", Location = new Point(125, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnXoa = new Button { Text = "Xóa", Location = new Point(230, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(335, 110), Width = 95, Height = 32, BackColor = Color.White, ForeColor = Color.Black, FlatStyle = FlatStyle.System };

            btnThem.Click += (s, e) => Them();
            btnSua.Click += (s, e) => Sua();
            btnXoa.Click += (s, e) => Xoa();
            btnLamMoi.Click += (s, e) => ClearForm();

            grpInput.Controls.AddRange(new Control[] {
                lblMa, txtMaPB, lblTen, txtTenPB, lblMoTa, txtMoTa,
                btnThem, btnSua, btnXoa, btnLamMoi
            });

            // Grid (Nền trắng, chữ đen)
            dgvPhongBan = new DataGridView
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
            dgvPhongBan.DefaultCellStyle.BackColor = Color.White;
            dgvPhongBan.DefaultCellStyle.ForeColor = Color.Black;
            dgvPhongBan.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 245);
            dgvPhongBan.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvPhongBan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvPhongBan.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvPhongBan.EnableHeadersVisualStyles = false;
            dgvPhongBan.CellClick += DgvPhongBan_CellClick;

            this.Controls.Add(dgvPhongBan);
            this.Controls.Add(grpInput);
            this.Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var list = _bus.GetAll();
            dgvPhongBan.DataSource = null;
            dgvPhongBan.DataSource = list;
            FormatGrid();
        }

        private void FormatGrid()
        {
            if (dgvPhongBan.Columns["MaPB"] != null) dgvPhongBan.Columns["MaPB"].HeaderText = "Mã Phòng Ban";
            if (dgvPhongBan.Columns["TenPB"] != null) dgvPhongBan.Columns["TenPB"].HeaderText = "Tên Phòng Ban";
            if (dgvPhongBan.Columns["Mota"] != null) dgvPhongBan.Columns["Mota"].HeaderText = "Mô Tả Chức Năng";
        }

        private void DgvPhongBan_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvPhongBan.Rows[e.RowIndex].DataBoundItem is PhongBanDTO pb)
            {
                txtMaPB.Text = pb.MaPB.ToString();
                txtTenPB.Text = pb.TenPB;
                txtMoTa.Text = pb.Mota;
            }
        }

        private void ClearForm()
        {
            txtMaPB.Clear();
            txtTenPB.Clear();
            txtMoTa.Clear();
            dgvPhongBan.ClearSelection();
        }

        private void Them()
        {
            var pb = new PhongBanDTO
            {
                TenPB = txtTenPB.Text.Trim(),
                Mota = txtMoTa.Text.Trim()
            };
            var (success, msg) = _bus.Add(pb);
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Sua()
        {
            if (!int.TryParse(txtMaPB.Text, out int maPB))
            {
                MessageBox.Show("Vui lòng chọn phòng ban cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pb = new PhongBanDTO
            {
                MaPB = maPB,
                TenPB = txtTenPB.Text.Trim(),
                Mota = txtMoTa.Text.Trim()
            };
            var (success, msg) = _bus.Update(pb);
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private void Xoa()
        {
            if (!int.TryParse(txtMaPB.Text, out int maPB))
            {
                MessageBox.Show("Vui lòng chọn phòng ban cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa phòng ban #{maPB} không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, msg) = _bus.Delete(maPB);
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
