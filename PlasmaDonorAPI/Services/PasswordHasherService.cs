using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto.Generators;
using BCrypt.Net;


namespace NewPlasmaDonorsAPI.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordHasherService()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            try
            {
                // Use BCrypt.Net to verify the password against the hash
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);

                return isPasswordValid;
            }
            catch (Exception ex)
            {
                // Handle any errors, like an invalid bcrypt hash format
                Console.WriteLine($"Error verifying password: {ex.Message}");
                return false;
            }
           

        }
    }
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}

