using System;
using System.Linq;
using System.Windows.Forms;
using VetClinic.Data;
using VetClinic.Data.Models;

namespace VetClinic.Forms
{
    /// <summary>
    /// Full CRUD form for Addresses.
    /// </summary>
    public class AddressesForm : Form
    {
        private readonly VetClinicContext _db = new();

        // ── Controls ───────────────────────────────────────────────────────────
        private DataGridView dgvAddresses = null!;
        private TextBox txtCountry = null!;
        private TextBox txtCity = null!;
        private TextBox txtStreet = null!;
        private TextBox txtPostal = null!;
        private Button btnAdd = null!;
        private Button btnUpdate = null!;
        private Button btnDelete = null!;
        private Button btnClear = null!;

        private int _selectedId = 0;

        public AddressesForm()
        {
            InitializeComponent();
            LoadGrid();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Addresses";
            this.Size = new System.Drawing.Size(700, 470);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.WhiteSmoke;

            // ── Grid ───────────────────────────────────────────────────────────
            dgvAddresses = new DataGridView
            {
                Location = new System.Drawing.Point(15, 15),
                Size = new System.Drawing.Size(660, 220),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = System.Drawing.Color.White
            };
            dgvAddresses.SelectionChanged += DgvAddresses_SelectionChanged;

            // ── Input fields ───────────────────────────────────────────────────
            int y = 250;
            this.Controls.Add(MakeLabel("Country:", 15, y)); txtCountry = MakeTextBox(90, y); this.Controls.Add(txtCountry);
            this.Controls.Add(MakeLabel("City:", 290, y)); txtCity = MakeTextBox(330, y); this.Controls.Add(txtCity);

            y += 40;
            this.Controls.Add(MakeLabel("Street:", 15, y)); txtStreet = MakeTextBox(80, y, 330); this.Controls.Add(txtStreet);
            this.Controls.Add(MakeLabel("Postal Code:", 430, y)); txtPostal = MakeTextBox(525, y, 130); this.Controls.Add(txtPostal);

            // ── Buttons ────────────────────────────────────────────────────────
            y += 55;
            btnAdd = MakeButton("Add", 15, y, System.Drawing.Color.FromArgb(0, 153, 76));
            btnUpdate = MakeButton("Update", 130, y, System.Drawing.Color.FromArgb(0, 120, 215));
            btnDelete = MakeButton("Delete", 245, y, System.Drawing.Color.FromArgb(200, 50, 50));
            btnClear = MakeButton("Clear", 360, y, System.Drawing.Color.Gray);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearInputs();

            this.Controls.AddRange(new Control[] { dgvAddresses, btnAdd, btnUpdate, btnDelete, btnClear });
        }

        // ── Data loading ───────────────────────────────────────────────────────
        private void LoadGrid()
        {
            dgvAddresses.DataSource = _db.Addresses.OrderBy(a => a.Country).ThenBy(a => a.City).ToList();

            if (dgvAddresses.Columns.Count > 0)
            {
                dgvAddresses.Columns["Id"].HeaderText = "ID";
                dgvAddresses.Columns["Country"].HeaderText = "Country";
                dgvAddresses.Columns["City"].HeaderText = "City";
                dgvAddresses.Columns["Street"].HeaderText = "Street";
                dgvAddresses.Columns["PostalCode"].HeaderText = "Postal Code";
                // Hide the reverse-navigation column if EF added it
                if (dgvAddresses.Columns.Contains("Owner"))
                    dgvAddresses.Columns["Owner"].Visible = false;
            }
        }

        // ── Row selected → populate inputs ────────────────────────────────────
        private void DgvAddresses_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvAddresses.SelectedRows.Count == 0) return;
            var row = dgvAddresses.SelectedRows[0];
            _selectedId = (int)row.Cells["Id"].Value;

            var addr = _db.Addresses.Find(_selectedId);
            if (addr == null) return;

            txtCountry.Text = addr.Country;
            txtCity.Text = addr.City;
            txtStreet.Text = addr.Street;
            txtPostal.Text = addr.PostalCode;
        }

        // ── CRUD handlers ──────────────────────────────────────────────────────
        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            _db.Addresses.Add(new Address
            {
                Country = txtCountry.Text.Trim(),
                City = txtCity.Text.Trim(),
                Street = txtStreet.Text.Trim(),
                PostalCode = txtPostal.Text.Trim()
            });
            _db.SaveChanges();
            ClearInputs();
            LoadGrid();
            MessageBox.Show("Address added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (_selectedId == 0) { MessageBox.Show("Select a row first."); return; }
            if (!ValidateInputs()) return;

            var addr = _db.Addresses.Find(_selectedId);
            if (addr == null) return;

            addr.Country = txtCountry.Text.Trim();
            addr.City = txtCity.Text.Trim();
            addr.Street = txtStreet.Text.Trim();
            addr.PostalCode = txtPostal.Text.Trim();

            _db.SaveChanges();
            ClearInputs();
            LoadGrid();
            MessageBox.Show("Address updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_selectedId == 0) { MessageBox.Show("Select a row first."); return; }

            // Warn if an owner is linked to this address
            var linked = _db.Owners.Any(o => o.AddressId == _selectedId);
            string msg = linked
                ? "An owner is linked to this address. Their address will be cleared. Delete anyway?"
                : "Delete this address?";

            if (MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var addr = _db.Addresses.Find(_selectedId);
            if (addr == null) return;

            _db.Addresses.Remove(addr);
            _db.SaveChanges();
            ClearInputs();
            LoadGrid();
        }

        // ── Helpers ────────────────────────────────────────────────────────────
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtCountry.Text)) { MessageBox.Show("Country is required."); return false; }
            if (string.IsNullOrWhiteSpace(txtCity.Text)) { MessageBox.Show("City is required."); return false; }
            if (string.IsNullOrWhiteSpace(txtStreet.Text)) { MessageBox.Show("Street is required."); return false; }
            if (string.IsNullOrWhiteSpace(txtPostal.Text)) { MessageBox.Show("Postal Code is required."); return false; }
            return true;
        }

        private void ClearInputs()
        {
            txtCountry.Clear(); txtCity.Clear(); txtStreet.Clear(); txtPostal.Clear();
            _selectedId = 0;
            dgvAddresses.ClearSelection();
        }

        private static Label MakeLabel(string text, int x, int y) =>
            new Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };

        private static TextBox MakeTextBox(int x, int y, int width = 180) =>
            new TextBox { Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(width, 25) };

        private static Button MakeButton(string text, int x, int y, System.Drawing.Color color) =>
            new Button
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(100, 35),
                BackColor = color,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };

        protected override void OnFormClosed(FormClosedEventArgs e) { _db.Dispose(); base.OnFormClosed(e); }
    }
}