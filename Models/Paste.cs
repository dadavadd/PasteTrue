using System.Security.Cryptography;
using System.Text;

namespace PasteTrue.Models
{
    public class Paste
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public string? UserId { get; set; }
        public virtual User User { get; set; }

        public string? PasswordHash { get; set; }

        public bool IsPublic { get; set; }



        public void SetPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    PasswordHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
            }
        }
        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(PasswordHash))
                return true;
            if (password == null)
                return false;

            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hashedPassword == PasswordHash;
            }
        }
    }
}
