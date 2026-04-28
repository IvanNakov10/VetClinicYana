using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;
using VetClinic.Data.Models;

namespace VetClinic.Forms
{
    /// <summary>
    /// Full CRUD form for Animals.
    /// </summary>
    public class AnimalsForm : Form
    {
        private readonly VetClinicContext _db = new();

        // ── Controls ───────────────────────────────────────────────────────────
        private DataGridView dgvAnimals = null!;
        private TextBox txtName = null!;
        private TextBox txtSpecies = null!;
        private TextBox txtBreed = null!;
        private TextBox txtAge = null!;
        private TextBox txtNotes = null!;
        private ComboBox cmbOwner = null!;
        private Button btnAdd = null!;
        private Button btnUpdate = null!;
        private Button btnDelete = null!;
        private Button btnClear = null!;

        private int _selectedId = 0;

        public AnimalsForm()
        {
            InitializeComponent();
            LoadOwnerCombo();
            LoadGrid();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Animals";
            this.Size = new System.Drawing.Size(800, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.WhiteSmoke;

            // ── Grid ───────────────────────────────────────────────────────────
            dgvAnimals = new DataGridView
            {
                Location = new System.Drawing.Point(15, 15),
                Size = new System.Drawing.Size(760, 230),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = System.Drawing.Color.White
            };
            dgvAnimals.SelectionChanged += DgvAnimals_SelectionChanged;

            // ── Row 1 inputs ───────────────────────────────────────────────────
            int y = 260;
            this.Controls.Add(MakeLabel("Name:", 15, y)); txtName = MakeTextBox(80, y); this.Controls.Add(txtName);
            this.Controls.Add(MakeLabel("Species:", 240, y)); txtSpecies = MakeTextBox(310, y, 140); this.Controls.Add(txtSpecies);
            this.Controls.Add(MakeLabel("Breed:", 470, y)); txtBreed = MakeTextBox(525, y, 235); this.Controls.Add(txtBreed);

            y += 40;
            this.Controls.Add(MakeLabel("Age:", 15, y)); txtAge = MakeTextBox(60, y, 60); this.Controls.Add(txtAge);
            this.Controls.Add(MakeLabel("Owner:", 140, y));
            cmbOwner = new ComboBox
            {
                Location = new System.Drawing.Point(200, y),
                Size = new System.Drawing.Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbOwner);

            y += 40;
            this.Controls.Add(MakeLabel("Medical Notes:", 15, y));
            txtNotes = new TextBox
            {
                Location = new System.Drawing.Point(115, y),
                Size = new System.Drawing.Size(645, 55),
                Multiline = true
            };
            this.Controls.Add(txtNotes);

            // ── Buttons ────────────────────────────────────────────────────────
            y += 70;
            btnAdd = MakeButton("Add", 15, y, System.Drawing.Color.FromArgb(0, 153, 76));
            btnUpdate = MakeButton("Update", 130, y, System.Drawing.Color.FromArgb(0, 120, 215));
            btnDelete = MakeButton("Delete", 245, y, System.Drawing.Color.FromArgb(200, 50, 50));
            btnClear = MakeButton("Clear", 360, y, System.Drawing.Color.Gray);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearInputs();

            this.Controls.AddRange(new Control[] { dgvAnimals, btnAdd, btnUpdate, btnDelete, btnClear });
        }

        // ── Data loading ───────────────────────────────────────────────────────
        private void LoadGrid()
        {
            var animals = _db.Animals
                             .Include(a => a.Owner)
                             .OrderBy(a => a.Name)
                             .Select(a => new
                             {
                                 a.Id,
                                 a.Name,
                                 a.Species,
                                 a.Breed,
                                 a.Age,
                                 Owner = a.Owner.FullName,
                                 MedicalNotes = a.MedicalNotes ?? "—"
                             })
                             .ToList();

            dgvAnimals.DataSource = animals;

            if (dgvAnimals.Columns.Count > 0)
            {
                dgvAnimals.Columns["Id"].HeaderText = "ID";
                dgvAnimals.Columns["Name"].HeaderText = "Name";
                dgvAnimals.Columns["Species"].HeaderText = "Species";
                dgvAnimals.Columns["Breed"].HeaderText = "Breed";
                dgvAnimals.Columns["Age"].HeaderText = "Age";
                dgvAnimals.Columns["Owner"].HeaderText = "Owner";
                dgvAnimals.Columns["MedicalNotes"].HeaderText = "Medical Notes";
            }
        }

        private void LoadOwnerCombo()
        {
            cmbOwner.DataSource = _db.Owners.Select(o => new { o.Id, o.FullName }).ToList();
            cmbOwner.DisplayMember = "FullName";
            cmbOwner.ValueMember = "Id";
            cmbOwner.SelectedIndex = -1;
        }

        // ── Row selected → populate inputs ────────────────────────────────────
        private void DgvAnimals_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0) return;
            var row = dgvAnimals.SelectedRows[0];
            _selectedId = (int)row.Cells["Id"].Value;

            var animal = _db.Animals.Find(_selectedId);
            if (animal == null) return;

            txtName.Text = animal.Name;
            txtSpecies.Text = animal.Species;
            txtBreed.Text = animal.Breed;
            txtAge.Text = animal.Age.ToString();
            txtNotes.Text = animal.MedicalNotes ?? "";
            cmbOwner.SelectedValue = animal.OwnerId;
        }

        // ── CRUD handlers ──────────────────────────────────────────────────────
        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            _db.Animals.Add(new Animal
            {
                Name = txtName.Text.Trim(),
                Species = txtSpecies.Text.Trim(),
                Breed = txtBreed.Text.Trim(),
                Age = int.Parse(txtAge.Text.Trim()),
                MedicalNotes = txtNotes.Text.Trim(),
                OwnerId = (int)cmbOwner.SelectedValue!
            });
            _db.SaveChanges();
            ClearInputs();
            LoadGrid();
            MessageBox.Show("Animal added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (_selectedId == 0) { MessageBox.Show("Select a row first."); return; }
            if (!ValidateInputs()) return;

            var animal = _db.Animals.Find(_selectedId);
            if (animal == null) return;

            animal.Name = txtName.Text.Trim();
            animal.Species = txtSpecies.Text.Trim();
            animal.Breed = txtBreed.Text.Trim();
            animal.Age = int.Parse(txtAge.Text.Trim());
            animal.MedicalNotes = txtNotes.Text.Trim();
            animal.OwnerId = (int)cmbOwner.SelectedValue!;

            _db.SaveChanges();
            ClearInputs();
            LoadGrid();
            MessageBox.Show("Animal updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_selectedId == 0) { MessageBox.Show("Select a row first."); return; }

            if (MessageBox.Show("Delete this animal?", "Confirm", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning) != DialogResult.Yes) return;

            var animal = _db.Animals.Find(_selectedId);
            if (animal == null) return;

            _db.Animals.Remove(animal);
            _db.SaveChanges();
            ClearInputs();
            LoadGrid();
        }

        // ── Helpers ────────────────────────────────────────────────────────────
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Name is required."); return false; }
            if (string.IsNullOrWhiteSpace(txtSpecies.Text)) { MessageBox.Show("Species is required."); return false; }
            if (string.IsNullOrWhiteSpace(txtBreed.Text)) { MessageBox.Show("Breed is required."); return false; }
            if (!int.TryParse(txtAge.Text, out int age) || age < 0)
            { MessageBox.Show("Enter a valid age (0 or more)."); return false; }
            if (cmbOwner.SelectedValue == null) { MessageBox.Show("Select an owner."); return false; }
            return true;
        }

        private void ClearInputs()
        {
            txtName.Clear(); txtSpecies.Clear(); txtBreed.Clear();
            txtAge.Clear(); txtNotes.Clear();
            cmbOwner.SelectedIndex = -1;
            _selectedId = 0;
            dgvAnimals.ClearSelection();
        }

        private static Label MakeLabel(string text, int x, int y) =>
            new Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };

        private static TextBox MakeTextBox(int x, int y, int width = 160) =>
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