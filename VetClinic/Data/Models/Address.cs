namespace VetClinic.Data.Models
{
    /// <summary>
    /// Represents a physical address linked to an Owner (one-to-one).
    /// </summary>
    public class Address
    {
        public int Id { get; set; }

        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        // Navigation property: one Address → one Owner
        public ICollection<Owner> Owners { get; set; } = new List<Owner>(); 
    }
}