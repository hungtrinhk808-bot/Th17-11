using System;
using System.Drawing;
using System.Windows.Forms;

namespace HRMPayrollSystem.GUI
{
    public class MainForm : Form
    {
        private Panel pnlSidebar = null!;
        private Panel pnlHeader = null!;
        private Panel pnlContent = null!;
        private Label lblHeaderTitle = null!;

        private Button btnNavNhanVien = null!;
        private Button btnNavPhongBan = null!;
        private Button btnNavChamCong = null!;
        private Button btnNavTinhLuong = null!;

        private Form? _currentChildForm = null;

        public MainForm()
        {
            InitializeComponent();
            OpenChildForm(new FormNhanVien(), "QUẢN LÝ HỒ SƠ NHÂN SỰ & ĐIỀU CHUYỂN PHÒNG BAN");
        }

        private void InitializeComponent()
        {
            this.Text = "HỆ THỐNG QUẢN LÝ NHÂN SỰ & TÍNH LƯƠNG (HRM & PAYROLL SYSTEM)";
            this.Size = new Size(1360, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            // Header (Nền trắng, chữ đen, viền)
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15),
                BorderStyle = BorderStyle.FixedSingle
            };
            lblHeaderTitle = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ NHÂN SỰ & TÍNH LƯƠNG (HRM & PAYROLL)",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = true
            };
            Label lblSubInfo = new Label
            {
                Text = "Disconnected Model • SqlTransaction ACID • Parameterized Query",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                Dock = DockStyle.Right,
                AutoSize = true
            };
            pnlHeader.Controls.AddRange(new Control[] { lblHeaderTitle, lblSubInfo });

            // Sidebar (Rộng 280px)
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = Color.White,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblMenu = new Label
            {
                Text = "DANH MỤC CHỨC NĂNG",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnNavNhanVien = CreateNavButton("👥   Hồ Sơ Nhân Sự", () => OpenChildForm(new FormNhanVien(), "QUẢN LÝ HỒ SƠ NHÂN SỰ & ĐIỀU CHUYỂN PHÒNG BAN"));
            btnNavPhongBan = CreateNavButton("🏢   Phòng Ban", () => OpenChildForm(new FormPhongBan(), "QUẢN LÝ DANH MỤC PHÒNG BAN"));
            btnNavChamCong = CreateNavButton("⏱️   Chấm Công (Disconnected)", () => OpenChildForm(new FormChamCong(), "CHẤM CÔNG THÁNG (Disconnected Model: DataSet / SqlDataAdapter)"));
            btnNavTinhLuong = CreateNavButton("💰   Tính & Chốt Lương (ACID)", () => OpenChildForm(new FormTinhLuong(), "TÍNH LƯƠNG & CHỐT LƯƠNG THÁNG (SqlTransaction)"));

            pnlSidebar.Controls.Add(btnNavTinhLuong);
            pnlSidebar.Controls.Add(btnNavChamCong);
            pnlSidebar.Controls.Add(btnNavPhongBan);
            pnlSidebar.Controls.Add(btnNavNhanVien);
            pnlSidebar.Controls.Add(lblMenu);

            // Content Panel (Nền trắng)
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlHeader);
        }

        private Button CreateNavButton(string text, Action onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 52,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, BorderColor = Color.LightGray },
                ForeColor = Color.Black,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.MouseEnter += (s, e) => { if (btn.BackColor != Color.FromArgb(230, 240, 255)) btn.BackColor = Color.FromArgb(245, 245, 245); };
            btn.MouseLeave += (s, e) => { if (btn.BackColor != Color.FromArgb(230, 240, 255)) btn.BackColor = Color.White; };
            btn.Click += (s, e) =>
            {
                ResetNavButtonsColor();
                btn.BackColor = Color.FromArgb(230, 240, 255);
                btn.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                onClick();
            };
            return btn;
        }

        private void ResetNavButtonsColor()
        {
            foreach (Control c in pnlSidebar.Controls)
            {
                if (c is Button b)
                {
                    b.BackColor = Color.White;
                    b.ForeColor = Color.Black;
                    b.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
                }
            }
        }

        private void OpenChildForm(Form childForm, string title)
        {
            if (_currentChildForm != null)
            {
                _currentChildForm.Close();
                _currentChildForm.Dispose();
            }

            _currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            childForm.BackColor = Color.White;
            childForm.ForeColor = Color.Black;

            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(childForm);
            pnlContent.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

            lblHeaderTitle.Text = title;
        }
    }
}
