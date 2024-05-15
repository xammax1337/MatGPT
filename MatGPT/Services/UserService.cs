using MatGPT.Models;
using System.Security.Claims;

namespace MatGPT.Services
{
    public class UserService
    {
        public int GetUserIdFromToken(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return -1; // Send the -1 if no user is found.
        }
    }
}
