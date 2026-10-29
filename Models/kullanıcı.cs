namespace StajEvrakSistemi.Models;

public abstract class User{
    public int Id { get; set; }
     public string İsim{ get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ŞifreHash { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string HomeUrl { get; set; } = string.Empty;
}