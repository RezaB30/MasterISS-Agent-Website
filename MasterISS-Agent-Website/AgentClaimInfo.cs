using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace MasterISS_Agent_Website
{
    public static class AgentClaimInfo
    {
        public static string UserEmail()
        {
            var email = CurrentClaims().Where(c => c.Type == ClaimTypes.Email)
                  .Select(c => c.Value).SingleOrDefault();

            return email;
        }

        public static string UserDisplayName()
        {
            var displayName = CurrentClaims().Where(c => c.Type == ClaimTypes.Name)
                  .Select(c => c.Value).SingleOrDefault();

            return displayName;
        }

        private static List<Claim> CurrentClaims()
        {
            var currentClaims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            return currentClaims;
        }
    }
}