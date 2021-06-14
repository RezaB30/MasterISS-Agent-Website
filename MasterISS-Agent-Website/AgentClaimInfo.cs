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

        public static string SetupServiceUser()
        {
            var setupServiceUser = CurrentClaims().Where(c => c.Type == "SetupServiceUser")
                  .Select(c => c.Value).SingleOrDefault();

            return setupServiceUser;
        }

        public static string SetupServiceHash()
        {
            var setupServiceHash = CurrentClaims().Where(c => c.Type == "SetupServiceHash")
                  .Select(c => c.Value).SingleOrDefault();

            return setupServiceHash;
        }

        public static int AgentId()
        {
            var agentId = CurrentClaims().Where(c => c.Type == "AgentId")
                  .Select(c => c.Value).FirstOrDefault();
            return Convert.ToInt32(agentId);
        }

        private static List<Claim> CurrentClaims()
        {
            var currentClaims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            return currentClaims;
        }
    }
}