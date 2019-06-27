using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoECommerceApp
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                (
                    "FlixOneStore.ReadAccess",
                    "FlixOneStore API",
                    new List<string>
                    {
                        JwtClaimTypes.Id,
                        JwtClaimTypes.Email,
                        JwtClaimTypes.Name,
                        JwtClaimTypes.GivenName,
                        JwtClaimTypes.FamilyName
                    }
                ),
                new ApiResource("FlixOneStore.FullAccess", "FlixOneStore API")
            };
        }//end GetApiResources

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    Enabled = true,
                    ClientName = "Html Page Client",
                    ClientId  = "htmlclient",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secretpassword".Sha256())
                    },

                    AllowedScopes = {"FlixOneStore.ReadAccess" }
                }
            };
        }
    }
}
