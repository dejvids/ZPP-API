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
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.SignIn
{
    public class SignInComponent : BaseComponent
    {
        [Inject]
        public SignInService SignInService { get; set; }
        public string Result { get; set; }
        public async Task SignInApp()
        {
            Console.WriteLine("Logowanie");
            var user = new LoginUser { Login = "dsurys", Password = "123456" };
            var content = new StringContent(Json.Serialize(user), System.Text.Encoding.UTF8, "application/json");
            var result = await Http.PostAsync(@"/api/sign-in", content);
            Console.WriteLine(result);
            if (SignInService is null)
            {
                Console.WriteLine("Sign in null");
                return;
            }
            if(result == null)
            {
                Console.WriteLine("Result null");
                return;
            }
            bool isOk = await SignInService.HandleSignIn(result);
            if(isOk)
                UriHelper.NavigateTo("/profil");
        }



        public void SignInFacebook()
        {
            Console.WriteLine("Facebook login");

            UriHelper.NavigateTo($"{AppCtx.BaseAddress}/sign-in-facebook");
            // await HandleSignIn(result);
        }

        public void SignInGoogle()
        {
            Console.WriteLine("Google login");
            UriHelper.NavigateTo($"{AppCtx.BaseAddress}/sign-in-google");
        }
    }
}
