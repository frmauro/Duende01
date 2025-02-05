﻿using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace Duende01
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource()
                {
                    Name = "verification",
                    UserClaims = new List<string>
                    {
                        JwtClaimTypes.Email,
                        JwtClaimTypes.EmailVerified
                    }
                }
            ];

        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new ApiScope(name: "productApi", displayName: "Product API"),
                new ApiScope(name: "cartApi", displayName: "Cart API"),
                new ApiScope(name: "couponApi", displayName: "Coupon API")
            ];

        public static IEnumerable<Client> Clients =>
            [
                   new Client
                    {
                        ClientId = "client",

                        // no interactive user, use the clientid/secret for authentication
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        // secret for authentication
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },

                        // scopes that client has access to
                        AllowedScopes = { "productApi", "cartApi", "couponApi" }
                    },
                    // interactive ASP.NET Core Web App
                    new Client
                    {
                        ClientId = "web",
                        ClientSecrets = { new Secret("secret".Sha256()) },

                        AllowedGrantTypes = GrantTypes.Code,
            
                        // where to redirect to after login
                        RedirectUris = { "https://localhost:5002/signin-oidc" },

                        // where to redirect to after logout
                        PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                        AllowOfflineAccess = true,

                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "verification",
                            "productApi",
                            "cartApi",
                            "couponApi"
                        }
                    }
            ];
    }
}