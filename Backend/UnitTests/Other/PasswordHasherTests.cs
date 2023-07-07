using BusinessLogic.Services;
using Entities;
using Microsoft.AspNetCore.Identity;
using OnlineChat;

namespace UnitTests.Other;

public class PasswordHasherTests
{
    [Fact]
    public void PasswordHasher_SuccessfullyVerifiesTruePassword()
    {
        const string username = "username";
        const string password = "password";
        // var user = new User(username, password);
        var hasher = new PasswordHasher();
        // var hashed = hasher.HashPassword(user, password);
        // user.Password = hashed;
        // var verificationResult = hasher.VerifyHashedPassword(user, hashedPassword: hashed, providedPassword: password);
        // Assert.Equal(verificationResult, PasswordVerificationResult.Success);
    }
}