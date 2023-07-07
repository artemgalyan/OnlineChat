using System.Security.Cryptography;
using System.Text;
using Constants;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services;

public class PasswordHasher : IPasswordHasher<User>
{
    public string HashPassword(User user, string password)
    {
        var salt = new byte[Security.SaltSize];
        Random.Shared.NextBytes(salt);
        var saltString = Convert.ToBase64String(salt);
        var hash = HashPassword(password, salt);
        return saltString + Convert.ToBase64String(hash);
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        var saltString = hashedPassword[..Security.InStringSaltSize];
        var actualHash = hashedPassword[Security.InStringSaltSize..];
        var salt = Convert.FromBase64String(saltString);

        var providedHash = HashPassword(providedPassword, salt);
        return providedHash.SequenceEqual(Convert.FromBase64String(actualHash))
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(password),
            salt: salt,
            iterations: Security.Iterations,
            hashAlgorithm: Security.HashAlgorithm,
            outputLength: Security.OutputLength
        );
    }
}