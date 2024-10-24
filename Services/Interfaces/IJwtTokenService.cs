using PasteTrue.Models;

namespace PasteTrue.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateToken(User user);
    }
}
