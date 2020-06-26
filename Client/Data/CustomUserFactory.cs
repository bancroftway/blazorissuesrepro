using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace blazorissuesrepro.Client.Data
{
    public class CustomUserFactory: AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        private const string AccountKey = "AccountKey";
        private readonly ILocalStorageService localStorage;
        public CustomUserFactory(IAccessTokenProviderAccessor accessor,
            ILocalStorageService localStorage)
            : base(accessor)
        {
            this.localStorage = localStorage;
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(
            RemoteUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            ClaimsPrincipal authUserAccount = null;
            try
            {
                authUserAccount = await base.CreateUserAsync(account, options);
                await SaveUserAccount(authUserAccount);
                return authUserAccount;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"exception occured in CreateUserAsync: {ex.Message}");
            }

            if (await localStorage.ContainKeyAsync(AccountKey))
            {
                Console.WriteLine("At position A");
                authUserAccount = await LoadUserAccount();

                if (authUserAccount.Identity.IsAuthenticated)
                {
                    Console.WriteLine("At position C");
                    var userIdentity = (ClaimsIdentity)authUserAccount.Identity;
                    Console.WriteLine($"At position C: {userIdentity.Name}");

                    return authUserAccount;
                }
            }

            return authUserAccount;
        }

        private async Task SaveUserAccount(ClaimsPrincipal user)
        {
            if (user != null)
            {
                //save this account to local storage
                await localStorage.SetItemAsync(AccountKey, user.Claims.Select(x => new ClaimData { Type = x.Type, Value = x.Value }).ToList());
            }
            else
            {
                await localStorage.RemoveItemAsync(AccountKey);
            }
        }

        private async Task<ClaimsPrincipal> LoadUserAccount()
        {
            var storedClaims = await localStorage.GetItemAsync<List<ClaimData>>(AccountKey);

            return storedClaims != null
                ? new ClaimsPrincipal(new ClaimsIdentity(storedClaims.Select(c => new Claim(c.Type, c.Value)), "appAuth"))
                : new ClaimsPrincipal(new ClaimsIdentity());
        }
    }

    internal class ClaimData
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
