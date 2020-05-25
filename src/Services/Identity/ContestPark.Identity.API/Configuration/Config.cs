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
                new ApiResource("balance", "Balance Service"),
                new ApiResource("chat", "Chat Service"),
                new ApiResource("duel", "Duel Service"),
                new ApiResource("follow", "Follow Service"),
                new ApiResource("signalr", "Duel Signalr"),
                new ApiResource("post", "Post Service"),
                new ApiResource("question", "Question Service"),
                new ApiResource("identity", "Identity Service"),
                new ApiResource("mobileshoppingagg", "Mobile Shopping Aggregator"),
                new ApiResource("admin", "Admin Service"),
                new ApiResource("notification", "Notification Service"),
                new ApiResource("mission", "Mission Service"),
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
                                        "balance",
                                        "duel",
                                        "chat",
                                        "follow",
                                        "mobileshoppingagg",
                                        "signalr",
                                        "post",
                                        "question",
                                        "identity",
                                        "notification",
                                        "mission",
                                    },
                        },
                 new Client
                        {
                            ClientId = "web-client",
                            ClientName = "ContestPark web client Client",
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
                                        "balance",
                                        "duel",
                                        "chat",
                                        "follow",
                                        "mobileshoppingagg",
                                        "signalr",
                                        "post",
                                        "question",
                                        "identity",
                                        "admin",
                                        "notification",
                                        "mission",
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
