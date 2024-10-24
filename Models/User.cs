using Microsoft.AspNetCore.Identity;

namespace PasteTrue.Models
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Paste> Pastes { get; set; }
    }
}
