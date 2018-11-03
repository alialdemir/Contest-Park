using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace ContestPark.Identity.API.Configuration
{
    public class Config
    {
        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("category", "Category Service"),
                new ApiResource("cp", "Cp Service"),
                new ApiResource("duel", "Duel Service"),
                new ApiResource("signalrhub", "Duel Signalr"),
                new ApiResource("question", "Question Service"),
                new ApiResource("mobileshoppingagg", "Mobile Shopping Aggregator"),
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                 new Client
                        {
                            ClientId = "xamarin",
                            ClientName = "ContestPark Xamarin Client",
                            AccessTokenLifetime = 3600 * 24 * 90,// 3 month
                            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                            RequireClientSecret = false,
                            AllowOfflineAccess = true,
                            AllowedScopes =new List<string>
                                    {
                                        IdentityServerConstants.StandardScopes.OpenId,
                                        IdentityServerConstants.StandardScopes.Profile,
                                        IdentityServerConstants.StandardScopes.OfflineAccess,
                                        "category",
                                        "cp",
                                        "duel",
                                        "mobileshoppingagg",
                                        "signalrhub",
                                        "question"
                                    },
                        },
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
    }
}