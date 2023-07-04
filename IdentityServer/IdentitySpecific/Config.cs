using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants;

namespace Project.IdentityServer.IdentitySpecific
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                    {
                        ClientId = "62bb3c20-e8a0-4284-85a0-4d8c99abc227",
                        ClientName = "web",
                        ClientSecrets = { new Secret("mz*cUsKEbmHUxIP!".Sha256()) },

                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        AllowedScopes = { "project.api", StandardScopes.OpenId, StandardScopes.OfflineAccess },
                        AllowOfflineAccess = true,
                        AccessTokenLifetime = 3600,
                        RefreshTokenExpiration = TokenExpiration.Absolute,
                        RefreshTokenUsage = TokenUsage.OneTimeOnly,
                        AbsoluteRefreshTokenLifetime = 1296000,

                        AlwaysIncludeUserClaimsInIdToken = true,
                        AccessTokenType = AccessTokenType.Reference
                    }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                // must match client beacuse of reference token
                new ApiResource
                {
                    Name= "62bb3c20-e8a0-4284-85a0-4d8c99abc227",
                    ApiSecrets = { new Secret("mz*cUsKEbmHUxIP!".Sha256()) },
                    Scopes = { "project.api", StandardScopes.OpenId, StandardScopes.OfflineAccess }
                }
            };
    }
}
