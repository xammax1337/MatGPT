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

        // New method that can be used for all endpoints to get UserId when needed.
        public int ValidateUserId(ClaimsPrincipal user)
        {
            // Gets the userId from the JWTtoken and checks if hte userId is not -1
            int userId = GetUserIdFromToken(user);
            if (userId == -1)
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }
            return userId;
        }
    }
}
