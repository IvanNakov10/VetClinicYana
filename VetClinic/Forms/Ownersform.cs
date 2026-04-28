using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;
using VetClinic.Data.Models;

namespace VetClinic.Forms
{
    /// <summary>
    /// Full CRUD form for Owners.
    /// Top half: DataGridView showing all owners.
    /// Bottom half: input fields + Add / Update / Delete / Clear buttons.
    /// </summary>
    public class OwnersForm : Form
    {
        // ── DB context ─────────────────────────────────────────────────────────
        private readonly VetClinicContext _db = new();

        // ── Controls ───────────────────────────────────────────────────────────
        private DataGridView dgvOwners = null!;
        private TextBox txtFullName = null!;
        private TextBox txtPhone = null!;
        private TextBox txtEmail = null!;
        private ComboBox cmbAddressId = null!;
        private Button btnAdd = null!;
        private Button btnUpdate = null!;
        private Button btnDelete = null!;
        private Button btnClear = null!;

        // Holds the ID of the row currently selected for edit/delete
        private int _selectedId = 0;

        public OwnersForm()
        {
            InitializeComponent();
            LoadAddressCombo();
            LoadGrid();
        }

        // ── UI setup ───────────────────────────────────────────────────────────
        private void InitializeComponent()
        {
            this.Text = "Manage Owners";
            this.Size = new System.Drawing.Size(750, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.WhiteSmoke;

            // ── Grid ───────────────────────────────────────────────────────────
            dgvOwners = new DataGridView
            {
                Location = new System.Drawing.Point(15, 15),
                Size = new System.Drawing.Size(710, 240),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = System.Drawing.Color.White
            };
            dgvOwners.SelectionChanged += DgvOwners_SelectionChanged;

            // ── Labels & inputs ────────────────────────────────────────────────
            int y = 270;
            this.Controls.Add(MakeLabel("Full Name:", 15, y));
            txtFullName = MakeTextBox(120, y); this.Controls.Add(txtFullName);

            this.Controls.Add(MakeLabel("Phone:", 320, y));
            txtPhone = MakeTextBox(390, y, 160); this.Controls.Add(txtPhone);

            y += 45;
            this.Controls.Add(MakeLabel("Email:", 15, y));
            txtEmail = MakeTextBox(120, y); this.Controls.Add(txtEmail);

            this.Controls.Add(MakeLabel("Address:", 320, y));
            cmbAddressId = new ComboBox
            {
                Location = new System.Drawing.Point(390, y),
                Size = new System.Drawing.Size(160, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbAddressId);

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

            this.Controls.AddRange(new Control[]
                { dgvOwners, btnAdd, btnUpdate, btnDelete, btnClear });
        }

        // ── Data loading ───────────────────────────────────────────────────────

        /// <summary>Reload the grid from the database.</summary>
        private void LoadGrid()
        {
            // Include the Address navigation property so we can show city
            var owners = _db.Owners
                            .Include(o => o.Address)
                            .OrderBy(o => o.FullName)
                            .Select(o => new
                            {
                                o.Id,
                                o.FullName,
                                o.PhoneNumber,
                                o.Email,
                                Address = o.Address != null
                                          ? $"{o.Address.City}, {o.Address.Country}"
                                          : "—"
                            })
                            .ToList();

            dgvOwners.DataSource = owners;

            // Friendly column headers
            if (dgvOwners.Columns.Count > 0)
            {
                dgvOwners.Columns["Id"].HeaderText = "ID";
                dgvOwners.Columns["FullName"].HeaderText = "Full Name";
                dgvOwners.Columns["PhoneNumber"].HeaderText = "Phone";
                dgvOwners.Columns["Email"].HeaderText = "Email";
                dgvOwners.Columns["Address"].HeaderText = "Address";
            }
        }

        /// <summary>Populate the address combo box.</summary>
        private void LoadAddressCombo()
        {
            cmbAddressId.DataSource = _db.Addresses
                                             .Select(a => new { a.Id, Display = $"{a.Street}, {a.City}" })
                                             .ToList();
            cmbAddressId.DisplayMember = "Display";
            cmbAddressId.ValueMember = "Id";
            cmbAddressId.SelectedIndex = -1;   // blank by default
        }

        // ── Grid row selected → populate inputs ───────────────────────────────
        private void DgvOwners_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvOwners.SelectedRows.Count == 0) return;

            var row = dgvOwners.SelectedRows[0];
            _selectedId = (int)row.Cells["Id"].Value;
            txtFullName.Text = row.Cells["FullName"].Value?.ToString();
            txtPhone.Text = row.Cells["PhoneNumber"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();

            // Reload full entity to get AddressId
            var owner = _db.Owners.Find(_selectedId);
            if (owner?.AddressId != null)
                cmbAddressId.SelectedValue = owner.AddressId;
            else
                cmbAddressId.SelectedIndex = -1;
        }

        // ── CRUD handlers ──────────────────────────────────────────────────────

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            var owner = new Owner
            {
                FullName = txtFullName.Text.Trim(),
                PhoneNumber = txtPhone.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                AddressId = cmbAddressId.SelectedValue as int?
            };

            _db.Owners.Add(owner);
            _db.SaveChanges();
            ClearInputs();
            LoadGrid();
            MessageBox.Show("Owner added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (_selectedId == 0) { MessageBox.Show("Select a row first."); return; }
            if (!ValidateInputs()) return;

            var owner = _db.Owners.Find(_selectedId);
            if (owner == null) return;

            owner.FullName = txtFullName.Text.Trim();
            owner.PhoneNumber = txtPhone.Text.Trim();
            owner.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
            owner.AddressId = cmbAddressId.SelectedValue as int?;

            _db.SaveChanges();
            ClearInputs();
            LoadGrid();
            MessageBox.Show("Owner updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_selectedId == 0) { MessageBox.Show("Select a row first."); return; }

            var confirm = MessageBox.Show(
                "Delete this owner and all their animals?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            var owner = _db.Owners.Include(o => o.Animals).FirstOrDefault(o => o.Id == _selectedId);
            if (owner == null) return;

            // EF Cascade will remove animals too (configured in OnModelCreating)
            _db.Owners.Remove(owner);
            _db.SaveChanges();
            ClearInputs();
            LoadGrid();
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            { MessageBox.Show("Full Name is required."); return false; }
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            { MessageBox.Show("Phone is required."); return false; }
            return true;
        }

        private void ClearInputs()
        {
            txtFullName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            cmbAddressId.SelectedIndex = -1;
            _selectedId = 0;
            dgvOwners.ClearSelection();
        }

        // ── Static UI factory helpers ──────────────────────────────────────────

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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _db.Dispose();
            base.OnFormClosed(e);
        }
    }
}