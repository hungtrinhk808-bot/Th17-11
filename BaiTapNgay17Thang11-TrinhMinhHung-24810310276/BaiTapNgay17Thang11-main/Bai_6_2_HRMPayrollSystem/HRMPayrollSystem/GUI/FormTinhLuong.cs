using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HRMPayrollSystem.BUS;
using HRMPayrollSystem.DTO;

namespace HRMPayrollSystem.GUI
{
    public class FormTinhLuong : Form
    {
        private readonly LuongBUS _luongBUS = new LuongBUS();
        private readonly ChamCongBUS _chamCongBUS = new ChamCongBUS();

        private ComboBox cboThang = null!;
        private NumericUpDown numNam = null!;
        private Button btnTinhLuong = null!;
        private Button btnChotLuong = null!;
        private Button btnXemDaChot = null!;
        private Label lblTongQuyLuong = null!;
        private Label lblTongNhanVien = null!;
        private Label lblTrangThaiKhoa = null!;
        private DataGridView dgvLuong = null!;

        private List<BangLuongDTO> _currentList = new List<BangLuongDTO>();

        public FormTinhLuong()
        {
            InitializeComponent();
            TinhLuong();
        }

        private void InitializeComponent()
        {
            this.Text = "Tính lương & Chốt lương tháng (SqlTransaction)";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 140, Padding = new Padding(15), BackColor = Color.White };

            Label lblTitle = new Label
            {
                Text = "TÍNH LƯƠNG & CHỐT LƯƠNG THÁNG (SqlTransaction ACID)",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 10)
            };

            Panel pnlFormula = new Panel
            {
                Location = new Point(18, 40),
                Size = new Size(1040, 32),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Label lblFormula = new Label
            {
                Text = "📌 CÔNG THỨC:  Thực Lĩnh = (Lương Cơ Bản × Hệ Số Lương / 22) × Số Ngày Công + (Số Giờ OT × 50,000 VNĐ)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlFormula.Controls.Add(lblFormula);

            // Controls
            Label lblThang = new Label { Text = "Tháng:", Location = new Point(18, 92), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.Black };
            cboThang = new ComboBox { Location = new Point(70, 88), Width = 70, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.White, ForeColor = Color.Black };
            for (int i = 1; i <= 12; i++) cboThang.Items.Add(i.ToString("D2"));
            cboThang.SelectedItem = DateTime.Today.Month.ToString("D2");

            Label lblNam = new Label { Text = "Năm:", Location = new Point(150, 92), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.Black };
            numNam = new NumericUpDown { Location = new Point(195, 88), Width = 80, Minimum = 2020, Maximum = 2050, Value = DateTime.Today.Year, BackColor = Color.White, ForeColor = Color.Black };

            btnTinhLuong = new Button
            {
                Text = "⚡ Tính Lương Dự Kiến",
                Location = new Point(290, 85),
                Width = 170,
                Height = 35,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                FlatStyle = FlatStyle.System
            };
            btnTinhLuong.Click += (s, e) => TinhLuong();

            btnChotLuong = new Button
            {
                Text = "🔒  CHỐT LƯƠNG THÁNG (SqlTransaction)",
                Location = new Point(475, 85),
                Width = 280,
                Height = 35,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.System
            };
            btnChotLuong.Click += (s, e) => ChotLuong();

            btnXemDaChot = new Button
            {
                Text = "📋 Xem Bảng Lương Đã Chốt",
                Location = new Point(770, 85),
                Width = 200,
                Height = 35,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                FlatStyle = FlatStyle.System
            };
            btnXemDaChot.Click += (s, e) => XemDaChot();

            pnlTop.Controls.AddRange(new Control[] {
                lblTitle, pnlFormula,
                lblThang, cboThang, lblNam, numNam,
                btnTinhLuong, btnChotLuong, btnXemDaChot
            });

            // Bottom Summary Panel (Nền trắng, chữ đen)
            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 65, Padding = new Padding(15), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            lblTongNhanVien = new Label
            {
                Text = "Tổng nhân sự: 0",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.Black
            };
            lblTongQuyLuong = new Label
            {
                Text = "TỔNG QUỸ LƯƠNG: 0 VNĐ",
                Location = new Point(250, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black
            };
            lblTrangThaiKhoa = new Label
            {
                Text = "",
                Location = new Point(750, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.Black
            };
            pnlBottom.Controls.AddRange(new Control[] { lblTongNhanVien, lblTongQuyLuong, lblTrangThaiKhoa });

            // Grid (Nền trắng, chữ đen)
            dgvLuong = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                ForeColor = Color.Black,
                GridColor = Color.LightGray
            };
            dgvLuong.DefaultCellStyle.BackColor = Color.White;
            dgvLuong.DefaultCellStyle.ForeColor = Color.Black;
            dgvLuong.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 245);
            dgvLuong.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvLuong.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvLuong.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvLuong.EnableHeadersVisualStyles = false;

            this.Controls.Add(dgvLuong);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(pnlTop);
        }

        private string GetThangNam()
        {
            return $"{cboThang.SelectedItem}/{numNam.Value}";
        }

        private void TinhLuong()
        {
            string thangNam = GetThangNam();
            _currentList = _luongBUS.TinhToanBangLuong(thangNam);

            dgvLuong.DataSource = null;
            dgvLuong.DataSource = _currentList;

            FormatGrid();
            CapNhatTongKet(thangNam, false);
        }

        private void XemDaChot()
        {
            string thangNam = GetThangNam();
            var list = _luongBUS.GetBangLuongDaChot(thangNam);

            if (list.Count == 0)
            {
                MessageBox.Show($"Tháng {thangNam} chưa được chốt lương!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _currentList = list;
            dgvLuong.DataSource = null;
            dgvLuong.DataSource = _currentList;

            FormatGrid();
            CapNhatTongKet(thangNam, true);
        }

        private void CapNhatTongKet(string thangNam, bool isOfficial)
        {
            bool isLocked = _chamCongBUS.IsMonthLocked(thangNam);
            lblTongNhanVien.Text = $"Tổng nhân sự: {_currentList.Count} người";

            decimal tong = _currentList.Sum(l => l.ThucLinh);
            lblTongQuyLuong.Text = $"TỔNG QUỸ LƯƠNG: {tong:N0} VNĐ";

            if (isLocked)
            {
                lblTrangThaiKhoa.Text = "🔒 Trạng thái: ĐÃ CHỐT LƯƠNG & KHÓA CHẤM CÔNG";
            }
            else
            {
                lblTrangThaiKhoa.Text = "🔓 Trạng thái: Chưa chốt (Bảng dự kiến)";
            }
        }

        private void FormatGrid()
        {
            if (dgvLuong.Columns["MaLuong"] != null) dgvLuong.Columns["MaLuong"].Visible = false;
            if (dgvLuong.Columns["MaNV"] != null) dgvLuong.Columns["MaNV"].HeaderText = "Mã NV";
            if (dgvLuong.Columns["HoTen"] != null) dgvLuong.Columns["HoTen"].HeaderText = "Họ và Tên";
            if (dgvLuong.Columns["TenPB"] != null) dgvLuong.Columns["TenPB"].HeaderText = "Phòng Ban";
            if (dgvLuong.Columns["ThangNam"] != null) dgvLuong.Columns["ThangNam"].HeaderText = "Tháng";
            if (dgvLuong.Columns["LuongCoBan"] != null)
            {
                dgvLuong.Columns["LuongCoBan"].HeaderText = "Lương Cơ Bản";
                dgvLuong.Columns["LuongCoBan"].DefaultCellStyle.Format = "N0";
            }
            if (dgvLuong.Columns["HeSoLuong"] != null)
            {
                dgvLuong.Columns["HeSoLuong"].HeaderText = "Hệ Số";
                dgvLuong.Columns["HeSoLuong"].DefaultCellStyle.Format = "0.0#";
            }
            if (dgvLuong.Columns["SoNgayCong"] != null)
            {
                dgvLuong.Columns["SoNgayCong"].HeaderText = "Ngày Công";
                dgvLuong.Columns["SoNgayCong"].DefaultCellStyle.Format = "0.#";
            }
            if (dgvLuong.Columns["SoGioOT"] != null)
            {
                dgvLuong.Columns["SoGioOT"].HeaderText = "Giờ OT";
                dgvLuong.Columns["SoGioOT"].DefaultCellStyle.Format = "0.#";
            }
            if (dgvLuong.Columns["ThucLinh"] != null)
            {
                dgvLuong.Columns["ThucLinh"].HeaderText = "THỰC LĨNH (VNĐ)";
                dgvLuong.Columns["ThucLinh"].DefaultCellStyle.Format = "N0";
                dgvLuong.Columns["ThucLinh"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
        }

        private void ChotLuong()
        {
            string thangNam = GetThangNam();

            if (_currentList.Count == 0)
            {
                _currentList = _luongBUS.TinhToanBangLuong(thangNam);
            }

            if (_currentList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu nhân viên để chốt lương!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal tong = _currentList.Sum(l => l.ThucLinh);

            string confirmMsg = $"XÁC NHẬN CHỐT LƯƠNG THÁNG {thangNam}:\n\n" +
                                $"• Số lượng nhân viên: {_currentList.Count} người\n" +
                                $"• Tổng quỹ lương chi trả: {tong:N0} VNĐ\n\n" +
                                $"Hệ thống sẽ mở SqlTransaction để:\n" +
                                $"1. Ghi nhận bảng lương chi tiết vào CSDL (BangLuong)\n" +
                                $"2. Khóa dữ liệu chấm công tháng {thangNam} (DaKhoa = 1)\n\n" +
                                $"Bạn có chắc chắn muốn chốt lương không?";

            if (MessageBox.Show(confirmMsg, "Xác nhận chốt lương (SqlTransaction)", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bool success = _luongBUS.ChotLuongThang(thangNam, _currentList, out string msg);
                if (success)
                {
                    MessageBox.Show(
                        $"{msg}\n\nGiao dịch SqlTransaction COMMIT thành công! Dữ liệu chấm công đã được khóa an toàn.",
                        "Chốt lương thành công (SqlTransaction COMMIT)",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    XemDaChot();
                }
                else
                {
                    MessageBox.Show(
                        $"LỖI TRONG QUÁ TRÌNH CHỐT LƯƠNG (SqlTransaction ROLLBACK):\n\n{msg}",
                        "Lỗi Transaction Rollback",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
