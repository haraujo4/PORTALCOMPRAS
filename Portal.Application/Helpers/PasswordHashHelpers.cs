using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Application.Helpers
{
    public static class PasswordHashHelpers
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public static string GenerateRandomPassword()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(10);
        }
    }
}
