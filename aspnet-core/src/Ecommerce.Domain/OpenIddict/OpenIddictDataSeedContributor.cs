﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Ecommerce.OpenIddict;

/* Creates initial data that is needed to property run the application
 * and make client-to-server communication possible.
 */
public class OpenIddictDataSeedContributor(
    IConfiguration configuration,
    IAbpApplicationManager applicationManager,
    IOpenIddictScopeManager scopeManager,
    IPermissionDataSeeder permissionDataSeeder,
    IStringLocalizer<OpenIddictResponse> l)
    : IDataSeedContributor, ITransientDependency
{
    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopesAsync();
        await CreateApplicationsAsync();
    }

    private async Task CreateScopesAsync()
    {
        if (await scopeManager.FindByNameAsync("Ecommerce") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "Ecommerce",
                DisplayName = "Ecommerce API",
                Resources =
                {
                    "Ecommerce"
                }
            });
        }
        
        if (await scopeManager.FindByNameAsync("Ecommerce.Admin") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "Ecommerce.Admin",
                DisplayName = "Ecommerce Admin API",
                Resources =
                {
                    "Ecommerce.Admin"
                }
            });
        }
    }

    private async Task CreateApplicationsAsync()
    {
        var commonScopes = new List<string>
        {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        };
        
        var adminScopes = new List<string>();
        adminScopes.AddRange(commonScopes);
        adminScopes.Add("Ecommerce.Admin");

        var clientScopes = new List<string>();
        clientScopes.AddRange(commonScopes);
        clientScopes.Add("Ecommerce");

        var configurationSection = configuration.GetSection("OpenIddict:Applications");

        //Admin Client
        var webAdminClientId = configurationSection["Ecommerce_Admin:ClientId"];
        if (!webAdminClientId.IsNullOrWhiteSpace())
        {
            var adminWebClientRootUrl = configurationSection["Ecommerce_Admin:RootUrl"]?.TrimEnd('/');
            await CreateApplicationAsync(
                name: webAdminClientId,
                type: OpenIddictConstants.ClientTypes.Confidential,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Admin Application",
                secret: configurationSection["Ecommerce_Admin:ClientSecret"] ?? "1q2w3e*",
                grantTypes: new List<string> //Hybrid flow
                {
                    OpenIddictConstants.GrantTypes.Password,
                    OpenIddictConstants.GrantTypes.RefreshToken,
                    OpenIddictConstants.GrantTypes.Implicit
                },
                scopes: adminScopes,
                redirectUri: adminWebClientRootUrl,                
                clientUri: adminWebClientRootUrl,
                postLogoutRedirectUri: adminWebClientRootUrl
            );
        }

        // Web Client
        var webClientId  = configurationSection["Ecommerce_Web:ClientId"];
        if (!webClientId.IsNullOrWhiteSpace())
        {
            var webClientRootUrl  = configurationSection["Ecommerce_Web:RootUrl"].EnsureEndsWith('/');

            await CreateApplicationAsync(
                name: webClientId,
                type: OpenIddictConstants.ClientTypes.Confidential,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Web Application",
                secret: configurationSection["Ecommerce_Web:ClientSecret"] ?? "1q2w3e*",
                grantTypes: new List<string>
                {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.GrantTypes.Implicit
                },
                scopes: clientScopes,
                redirectUri: $"{webClientRootUrl}signin-oidc",
                clientUri: webClientRootUrl,
                postLogoutRedirectUri: $"{webClientRootUrl}signout-callback-oidc"
            );
        }
        
        // Swagger Client
        var swaggerClientId = configurationSection["Ecommerce_Admin_Swagger:ClientId"];
        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var swaggerRootUrl = configurationSection["Ecommerce_Admin_Swagger:RootUrl"]?.TrimEnd('/');
            await CreateApplicationAsync(
                name: swaggerClientId,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Swagger Admin Application",
                secret: null,
                grantTypes: new List<string>
                {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                },
                scopes: adminScopes,
                redirectUri: $"{swaggerRootUrl}/swagger/oauth2-redirect.html",
                clientUri: swaggerRootUrl
            );
        }
    }

    private async Task CreateApplicationAsync(
        [NotNull] string name,
        [NotNull] string type,
        [NotNull] string consentType,
        string displayName,
        string secret,
        List<string> grantTypes,
        List<string> scopes,
        string clientUri = null,
        string redirectUri = null,
        string postLogoutRedirectUri = null,
        List<string> permissions = null)
    {
        if (!string.IsNullOrEmpty(secret) && string.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException(l["NoClientSecretCanBeSetForPublicApplications"]);
        }

        if (string.IsNullOrEmpty(secret) && string.Equals(type, OpenIddictConstants.ClientTypes.Confidential, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException(l["TheClientSecretIsRequiredForConfidentialApplications"]);
        }

        if (!string.IsNullOrEmpty(name) && await applicationManager.FindByClientIdAsync(name) != null)
        {
            return;
            //throw new BusinessException(L["TheClientIdentifierIsAlreadyTakenByAnotherApplication"]);
        }

        var client = await applicationManager.FindByClientIdAsync(name);
        if (client is null)
        {
            var application = new AbpApplicationDescriptor
            {
                ClientId = name,
                Type = type,
                ClientSecret = secret,
                ConsentType = consentType,
                DisplayName = displayName,
                ClientUri = clientUri,
            };

            Check.NotNullOrEmpty(grantTypes, nameof(grantTypes));
            Check.NotNullOrEmpty(scopes, nameof(scopes));

            if (new [] { OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit }.All(grantTypes.Contains))
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);

                if (string.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
                }
            }

            if (!redirectUri.IsNullOrWhiteSpace() || !postLogoutRedirectUri.IsNullOrWhiteSpace())
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);
            }

            foreach (var grantType in grantTypes)
            {
                if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                }

                if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode || grantType == OpenIddictConstants.GrantTypes.Implicit)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                }

                if (grantType is OpenIddictConstants.GrantTypes.AuthorizationCode or OpenIddictConstants.GrantTypes.ClientCredentials ||
                    grantType == OpenIddictConstants.GrantTypes.Password ||
                    grantType == OpenIddictConstants.GrantTypes.RefreshToken ||
                    grantType == OpenIddictConstants.GrantTypes.DeviceCode)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Revocation);
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
                }

                if (grantType == OpenIddictConstants.GrantTypes.ClientCredentials)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
                }

                if (grantType == OpenIddictConstants.GrantTypes.Implicit)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
                }

                if (grantType == OpenIddictConstants.GrantTypes.Password)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
                }

                if (grantType == OpenIddictConstants.GrantTypes.RefreshToken)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                }

                if (grantType == OpenIddictConstants.GrantTypes.DeviceCode)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
                    application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
                }

                if (grantType == OpenIddictConstants.GrantTypes.Implicit)
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
                    if (string.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                    {
                        application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                        application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
                    }
                }
            }

            var buildInScopes = new []
            {
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Phone,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles
            };

            foreach (var scope in scopes)
            {
                if (buildInScopes.Contains(scope))
                {
                    application.Permissions.Add(scope);
                }
                else
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
                }
            }

            if (redirectUri != null)
            {
                if (!redirectUri.IsNullOrEmpty())
                {
                    if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                    {
                        throw new BusinessException(l["InvalidRedirectUri", redirectUri]);
                    }

                    if (application.RedirectUris.All(x => x != uri))
                    {
                        application.RedirectUris.Add(uri);
                    }
                }
            }

            if (postLogoutRedirectUri != null)
            {
                if (!postLogoutRedirectUri.IsNullOrEmpty())
                {
                    if (!Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                    {
                        throw new BusinessException(l["InvalidPostLogoutRedirectUri", postLogoutRedirectUri]);
                    }

                    if (application.PostLogoutRedirectUris.All(x => x != uri))
                    {
                        application.PostLogoutRedirectUris.Add(uri);
                    }
                }
            }

            if (permissions != null)
            {
                await permissionDataSeeder.SeedAsync(
                    ClientPermissionValueProvider.ProviderName,
                    name,
                    permissions,
                    null
                );
            }

            await applicationManager.CreateAsync(application);
        }
    }
}
