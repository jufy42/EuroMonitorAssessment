using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Library.Helpers
{
    public static class ClaimsExtentions 
    {
        private static async Task ReloadUser(this HttpContext context, ClaimsPrincipal currentPrincipal, AuthenticationProperties properties = null, string authSchema = null)
        {
            var result = await context.AuthenticateAsync();
            await context.SignOutAsync();
            await context.SignInAsync(authSchema ?? IdentityConstants.ApplicationScheme, currentPrincipal, properties ?? result.Properties);
        }

        /// <summary>
        /// Add or update a claim for an authenticated user
        /// </summary>
        /// <param name="context">The HttpContext of the site.</param>
        /// <param name="currentPrincipal">The principal user you are adding or updating the claim for.</param>
        /// <param name="key">The name of the claim.</param>
        /// <param name="value">The value of the claim.</param>
        /// <param name="properties">Any additional AuthenticationProperties as an object.</param>
        /// <param name="authSchema">The authentication schema.</param>
        public static async Task AddOrUpdateClaim(this HttpContext context, ClaimsPrincipal currentPrincipal, string key, string value, AuthenticationProperties properties = null, string authSchema = null)
        {
            if (!(currentPrincipal.Identity is ClaimsIdentity identity))
            {
                return;
            }

            var claim = identity.FindFirst(key);
            if (claim != null)
                identity.RemoveClaim(claim);

            identity.AddClaim(new Claim(key, value));

            await context.ReloadUser(currentPrincipal, properties, authSchema);
        }

        /// <summary>
        /// Add or update a list of claims for an authenticated user
        /// </summary>
        /// <param name="context">The HttpContext of the site.</param>
        /// <param name="currentPrincipal">The principal user you are adding or updating the claim for.</param>
        /// <param name="claims">The list of claims to add or update.</param>
        /// <param name="properties">Any additional AuthenticationProperties as an object.</param>
        /// <param name="authSchema">The authentication schema.</param>
        public static async Task AddOrUpdateClaims(this HttpContext context, ClaimsPrincipal currentPrincipal, Dictionary<string, string> claims, AuthenticationProperties properties = null, string authSchema = null)
        {
            if (!(currentPrincipal.Identity is ClaimsIdentity identity))
                return;

            foreach (var claim in claims)
            {
                var newClaim = identity.FindFirst(claim.Key);
                if (newClaim != null)
                    identity.RemoveClaim(newClaim);

                identity.AddClaim(new Claim(claim.Key, claim.Value));
            }

            await context.ReloadUser(currentPrincipal, properties, authSchema);
        }

        /// <summary>
        /// Remove a claim for an authenticated user
        /// </summary>
        /// <param name="context">The HttpContext of the site.</param>
        /// <param name="currentPrincipal">The principal user you are removing the claim from.</param>
        /// <param name="key">The name of the claim.</param>
        /// <param name="properties">Any additional AuthenticationProperties as an object.</param>
        /// <param name="authSchema">The authentication schema.</param>
        public static async Task RemoveClaim(this HttpContext context, ClaimsPrincipal currentPrincipal, string key, AuthenticationProperties properties = null, string authSchema = null)
        {
            if (!(currentPrincipal.Identity is ClaimsIdentity identity))
                return;

            var newClaim = identity.FindFirst(key);
            if (newClaim != null)
                identity.RemoveClaim(newClaim);

            await context.ReloadUser(currentPrincipal, properties, authSchema);
        }

        /// <summary>
        /// Remove a list of claims for an authenticated user
        /// </summary>
        /// <param name="context">The HttpContext of the site.</param>
        /// <param name="currentPrincipal">The principal user you are removing the claim from.</param>
        /// <param name="keys">The list of claim names.</param>
        /// <param name="properties">Any additional AuthenticationProperties as an object.</param>
        /// <param name="authSchema">The authentication schema.</param>
        public static async Task RemoveClaims(this HttpContext context, ClaimsPrincipal currentPrincipal, List<string> keys, AuthenticationProperties properties = null, string authSchema = null)
        {
            if (!(currentPrincipal.Identity is ClaimsIdentity identity))
                return;

            foreach (var key in keys)
            {
                var newClaim = identity.FindFirst(key);
                if (newClaim != null)
                    identity.RemoveClaim(newClaim);
            }

            await context.ReloadUser(currentPrincipal, properties, authSchema);
        }

        /// <summary>
        /// Gets the claim value based on the claim key and the user
        /// </summary>
        /// <param name="currentPrincipal">The principal user you are removing the claim from.</param>
        /// <param name="key">The name of the claim.</param>
        public static string GetClaimValue(this ClaimsPrincipal currentPrincipal, string key)
        {
            if (!(currentPrincipal.Identity is ClaimsIdentity identity))
                return null;

            var claim = identity.FindFirst(key);
            return claim?.Value;
        }

        /// <summary>
        /// Checks if a claim with a specific claim key exists for a user
        /// </summary>
        /// <param name="currentPrincipal">The principal user you are removing the claim from.</param>
        /// <param name="key">The name of the claim.</param>
        public static bool HasClaim(this ClaimsPrincipal currentPrincipal, string key)
        {
            if (!(currentPrincipal.Identity is ClaimsIdentity identity))
                return false;

            return identity.Claims.Any(m => m.Type == key);
        }
    }
}
