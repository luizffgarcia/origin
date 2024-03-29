namespace OriginDatabase.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Country { get; set; }
        public string? AccessType { get; set; }
        public int? EmployerId { get; set; }
        public DateTime? BirthDate { get; set; }
        public decimal? Salary { get; set; }
        public string? EmployerName { get; set; } // To associate with an employer when needed
    }
}
