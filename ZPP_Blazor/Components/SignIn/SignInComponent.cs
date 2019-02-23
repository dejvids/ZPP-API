using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Services;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components.SignIn
{
    public class SignInComponent :BaseComponent
    {
        public string Result { get; set; }
        public async void SignInApp()
        {
            Console.WriteLine("Logowanie");
            var user = new { Login = "dsurys", Password = "123456" };
            var content = new StringContent(Json.Serialize(user), System.Text.Encoding.UTF8, "application/json");
            var result = await Http.PostAsync(@"/api/sign-in", content);

            var response = await result.Content.ReadAsStringAsync();
            var obj = Json.Deserialize<SignInResult>(response);
            if (obj.Success)
            {
                AppCtx.AccessToken = obj.Token.AccessToken;
                await SessionStorage.SetItem<JsonWebToken>("accessToken", obj.Token);
                UriHelper.NavigateTo("/me");
            }
            else
                Console.WriteLine(obj.Message);
        }
    }
}
