
using System.Security.Cryptography;

namespace StajEvrakSistemi.Yardimcilar;

public static class Password
{
        public static string HashPassword(string şifre)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            şifre,
            salt,
            600000,
            HashAlgorithmName.SHA256,
            32
        );

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string şifre, string şifreHash)
    {
        string[] parts = şifreHash.Split(':');

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        byte[] tekrarHash = Rfc2898DeriveBytes.Pbkdf2(
            şifre,
            salt,
            600000,
            HashAlgorithmName.SHA256,
            32
        );

        return CryptographicOperations.FixedTimeEquals(
            hash,
            tekrarHash
        );
    }
}
