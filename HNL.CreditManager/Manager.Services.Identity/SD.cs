using System.Collections.Generic;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Manager.Services.Identity
{
    /// <summary>
    /// Clase para crear el metodo seed  
    /// </summary>
    public static class SD
    {

        //Creacion de dos roles
        public const string Admin = "Admin";
        public const string Customer = "Customer";


        /// <summary>
        /// Creacio de usuarios
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope> {
                new ApiScope("HNL", "HNL Server"),
                new ApiScope(name: "read",   displayName: "Read your data."),
                new ApiScope(name: "write",  displayName: "Write your data."),
                new ApiScope(name: "delete", displayName: "Delete your data.")
            };


        /// <summary>
        /// En produccion se debe poner un secret de llave
        /// </summary>
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                ///con esta seccion se crea un cliente generico
                new Client
                {
                    ClientId="client",
                    ClientSecrets= { new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes={ "read", "write","profile"}
                },

                ///creando un client para el proyecto HNL
                new Client
                {
                    ClientId="HNL",
                    ClientSecrets= { new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris={ "https://localhost:44380/signin-oidc" },
                    PostLogoutRedirectUris={"https://localhost:44380/signout-callback-oidc" },
                    AllowedScopes=new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "HNL"
                    }
                },
            };

    }
}
