using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using VideoGameCharacter.Data;

namespace VideoGameCharacter.Services
{
    //This class is called by the ASP.NET Core authentication middleware to transform the claims of the authenticated user.
    public class DbClaimsTransformation(AppDbContext context, ILogger<DbClaimsTransformation> logger) : IClaimsTransformation
    {
        //Checks if the user exists in the database and adds a custom claim for the user's role.
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            //Extracts the email embedded in the token by CreateToken. 
            var email = principal.FindFirstValue(ClaimTypes.Name);

            //If the email is missing, there is nothing to look up, so the principal is returned unchanged.
            if (email is null)
                return principal;

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            //Removes any Role claim already present on the token, since it will be replaced below with a freshly-queried value (or omitted entirely).
            var identity = (ClaimsIdentity)principal.Identity!;
            var existingRoleClaim = identity.FindFirst(ClaimTypes.Role);

            if (existingRoleClaim is not null)
                identity.RemoveClaim(existingRoleClaim);

            //If the User was deleted after the token was issued, no Role claim gets added, so any [Authorize(Roles = "Admin")] check downstream fails naturally.
            if (user is null)
            {
                logger.LogWarning($"{nameof(TransformAsync)}: Token presented for email '{email}', but no matching user exists.");
                return principal;
            }

            //If the User is not activeafter the token was issued, no Role claim gets added, so any [Authorize(Roles = "Admin")] check downstream fails naturally.
            if (!user.IsActive)
            {
                logger.LogWarning($"{nameof(TransformAsync)}: Token presented for inactive user '{email}'.");
                return principal;
            }

            //If the User exists and is active, attaches their current role from the database, which [Authorize(Roles = "Admin")] checks against on admin-only endpoints
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
            return principal;
        }
    }
}