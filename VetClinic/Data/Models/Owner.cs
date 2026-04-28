namespace VetClinic.Data.Models
{
    /// <summary>
    /// Represents a pet owner who can have one Address and many Animals.
    /// </summary>
    public class Owner
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }   // optional

        // Foreign key → Address (one-to-one)
        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        // Navigation property: one Owner → many Animals
        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}