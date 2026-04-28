using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VetClinic.Forms
{
    // ══════════════════════════════════════════════════════════════
    //  Dashboard – dark-themed navigation hub
    //  Each section opens its CRUD form as a modal dialog.
    // ══════════════════════════════════════════════════════════════
    public class DashboardForm : Form
    {
        // ── Palette ────────────────────────────────────────────────
        private static readonly Color BgDeep = Color.FromArgb(15, 17, 32);   // outer bg
        private static readonly Color BgCard = Color.FromArgb(28, 30, 53);   // nav cards
        private static readonly Color BgCardHover = Color.FromArgb(37, 40, 72);
        private static readonly Color AccentPurple = Color.FromArgb(122, 111, 224);
        private static readonly Color AccentTeal = Color.FromArgb(62, 201, 154);
        private static readonly Color AccentBlue = Color.FromArgb(90, 173, 238);
        private static readonly Color TextPrimary = Color.FromArgb(232, 230, 255);
        private static readonly Color TextMuted = Color.FromArgb(110, 106, 154);

        public DashboardForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Veterinary Clinic";
            this.Size = new Size(420, 500);
            this.MinimumSize = new Size(420, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = BgDeep;

            // ── Header panel ───────────────────────────────────────
            var pnlHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(420, 150),
                BackColor = BgDeep
            };

            // Logo circle
            var picLogo = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(180, 28),
                BackColor = Color.Transparent
            };
            picLogo.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                // Rounded square background
                using var bgBrush = new SolidBrush(Color.FromArgb(50, 45, 106));
                FillRoundedRect(g, bgBrush, 0, 0, 60, 60, 14);
                // Draw a simple paw shape using circles
                using var pawBrush = new SolidBrush(AccentPurple);
                g.FillEllipse(pawBrush, 8, 18, 13, 13);   // left toe
                g.FillEllipse(pawBrush, 24, 12, 13, 13);   // top toe
                g.FillEllipse(pawBrush, 40, 18, 13, 13);   // right toe
                // Main pad
                using var padBrush = new SolidBrush(AccentPurple);
                FillRoundedRect(g, padBrush, 10, 32, 40, 22, 10);
            };

            var lblAppName = new Label
            {
                Text = "Veterinary Clinic",
                Font = new Font("Segoe UI", 15, FontStyle.Regular),
                ForeColor = TextPrimary,
                AutoSize = false,
                Size = new Size(360, 30),
                Location = new Point(30, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text = "Management System  ·  Admin",
                Font = new Font("Segoe UI", 9),
                ForeColor = TextMuted,
                AutoSize = false,
                Size = new Size(360, 20),
                Location = new Point(30, 128),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.AddRange(new Control[] { picLogo, lblAppName, lblSub });

            // ── Nav cards ──────────────────────────────────────────
            var cardOwners = CreateNavCard(
                "Manage Owners",
                "View, add, edit and delete owners",
                AccentPurple,
                new Point(30, 160),
                () => new OwnersForm().ShowDialog(this));

            var cardAnimals = CreateNavCard(
                "Manage Animals",
                "Browse and manage patient records",
                AccentTeal,
                new Point(30, 255),
                () => new AnimalsForm().ShowDialog(this));

            var cardAddresses = CreateNavCard(
                "Manage Addresses",
                "Edit location and address records",
                AccentBlue,
                new Point(30, 350),
                () => new AddressesForm().ShowDialog(this));

            // ── Footer ─────────────────────────────────────────────
            var lblFooter = new Label
            {
                
            };

            this.Controls.AddRange(new Control[]
                { pnlHeader, cardOwners, cardAnimals, cardAddresses, lblFooter });
        }

        // ── Factory: rounded nav card ──────────────────────────────
        private Panel CreateNavCard(string title, string subtitle,
                                    Color accentColor, Point location,
                                    Action onClick)
        {
            var card = new Panel
            {
                Location = location,
                Size = new Size(355, 78),
                BackColor = BgCard,
                Cursor = Cursors.Hand
            };

            // Rounded border via Paint
            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(Color.FromArgb(46, 46, 80), 1f);
                DrawRoundedRect(g, pen, 0, 0, card.Width - 1, card.Height - 1, 10);
            };

            // Accent bar on the left
            var pnlAccent = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(4, 78),
                BackColor = accentColor
            };
            // Round only left corners via Paint
            pnlAccent.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var b = new SolidBrush(accentColor);
                FillRoundedRectLeft(g, b, 0, 0, 4, 78, 4);
            };

            // Icon dot
            var picDot = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(18, 19),
                BackColor = Color.Transparent
            };
            picDot.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var bg = new SolidBrush(Color.FromArgb(40, accentColor));
                FillRoundedRect(g, bg, 0, 0, 40, 40, 10);
                using var fg = new SolidBrush(accentColor);
                g.FillEllipse(fg, 14, 14, 12, 12);
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = TextPrimary,
                AutoSize = false,
                Size = new Size(250, 22),
                Location = new Point(70, 16),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 8),
                ForeColor = TextMuted,
                AutoSize = false,
                Size = new Size(250, 18),
                Location = new Point(70, 40),
                BackColor = Color.Transparent
            };

            // Arrow label
            var lblArrow = new Label
            {
                Text = "›",
                Font = new Font("Segoe UI", 18),
                ForeColor = Color.FromArgb(74, 70, 122),
                AutoSize = false,
                Size = new Size(24, 40),
                Location = new Point(320, 18),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.AddRange(new Control[]
                { pnlAccent, picDot, lblTitle, lblSub, lblArrow });

            // Hover effect — propagate to child controls too
            EventHandler enterHandler = (s, e) => card.BackColor = BgCardHover;
            EventHandler leaveHandler = (s, e) => card.BackColor = BgCard;
            MouseEventHandler clickHandler = (s, e) => onClick();

            card.MouseEnter += enterHandler;
            card.MouseLeave += leaveHandler;
            card.MouseClick += clickHandler;

            foreach (Control child in card.Controls)
            {
                child.MouseEnter += enterHandler;
                child.MouseLeave += leaveHandler;
                child.MouseClick += clickHandler;
                child.Cursor = Cursors.Hand;
            }

            return card;
        }

        // ── GDI helpers ────────────────────────────────────────────

        private static void FillRoundedRect(Graphics g, Brush b,
                                            int x, int y, int w, int h, int r)
        {
            using var path = RoundedPath(x, y, w, h, r);
            g.FillPath(b, path);
        }

        // Only round the left two corners (for accent bar)
        private static void FillRoundedRectLeft(Graphics g, Brush b,
                                                int x, int y, int w, int h, int r)
        {
            using var path = new GraphicsPath();
            path.AddArc(x, y, r * 2, r * 2, 180, 90);                 // top-left
            path.AddLine(x + w, y, x + w, y + h);                      // top-right straight
            path.AddLine(x + w, y + h, x + r, y + h);                  // bottom straight
            path.AddArc(x, y + h - r * 2, r * 2, r * 2, 90, 90);      // bottom-left
            path.CloseFigure();
            g.FillPath(b, path);
        }

        private static void DrawRoundedRect(Graphics g, Pen p,
                                            int x, int y, int w, int h, int r)
        {
            using var path = RoundedPath(x, y, w, h, r);
            g.DrawPath(p, path);
        }

        private static GraphicsPath RoundedPath(int x, int y, int w, int h, int r)
        {
            var path = new GraphicsPath();
            path.AddArc(x, y, r * 2, r * 2, 180, 90);
            path.AddArc(x + w - r * 2, y, r * 2, r * 2, 270, 90);
            path.AddArc(x + w - r * 2, y + h - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(x, y + h - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}