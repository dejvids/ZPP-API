using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Services
{
    public class SignInService
    {
        SessionStorage _sessionStorage;
        LocalStorage _localStorage;

        public SignInService(SessionStorage sessionStorage, LocalStorage localStorage)
        {
            _sessionStorage = sessionStorage;
            _localStorage = localStorage;
        }
        public async Task<bool> HandleSignIn(HttpResponseMessage result)
        {
            var response = await result.Content?.ReadAsStringAsync();
            if (response == null)
                return false;
            var obj = Json.Deserialize<SignInResult>(response);

            if (obj == null)
                return false;
            if(_sessionStorage == null)
            {
                Console.WriteLine("Sessionstorage is null");
                return false;
            }
            if (obj.Success)
            {
                await SetUserToken(obj.Token);
                return true;
            }
            Console.WriteLine(obj.Message);
            return false;
        }

        public async Task SetUserToken(JsonWebToken token)
        {
            AppCtx.AccessToken = token?.AccessToken;
            await _sessionStorage.SetItem<JsonWebToken>("token", token);
            await _localStorage.SetItem<JsonWebToken>("token", token);
        }
    }
}
