namespace StajEvrakSistemi.Models;

public class DatasetStudent
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string TcKimlikNo { get; set; } = "";
    public string Nationality { get; set; } = "";
    public string StudentNo { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public string Address { get; set; } = "";
    public string Department { get; set; } = "";
    public string Ek16Year { get; set; } = "";
    public int PreviousInternshipDays { get; set; }
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string HealthAidStatus { get; set; } = "";
}