using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components
{
    public class AppComponents : BaseComponent
    {
        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            var token = AppCtx.AccessToken ?? (await SessionStorage.GetItem<JsonWebToken>("accessToken"))?.AccessToken ?? (await LocalStorage.GetItem<JsonWebToken>("accessToken"))?.AccessToken;
            Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
