namespace VetClinic.Data.Models
{
    /// <summary>
    /// Represents an animal (pet) that belongs to an Owner.
    /// </summary>
    public class Animal
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Species { get; set; } = string.Empty;   // e.g. Dog, Cat

        public string Breed { get; set; } = string.Empty;

        public int Age { get; set; }

        public string? MedicalNotes { get; set; }

        // Foreign key → Owner (many-to-one)
        public int OwnerId { get; set; }
        public Owner Owner { get; set; } = null!;
    }
}