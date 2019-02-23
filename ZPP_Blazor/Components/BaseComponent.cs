using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ZPP_Blazor.Components
{
    public class BaseComponent : BlazorComponent
    {
        string _developBaseAddress = @"https://localhost:5001";
        [Inject]
        protected HttpClient Http { get; set; }
        [Inject]
        protected SessionStorage SessionStorage { get; set; }

        [Inject]
        protected LocalStorage LocalStorage { get; set; }

        [Inject]
        protected IUriHelper UriHelper { get; set; }

        protected override Task OnInitAsync()
        {
#if DEBUG
            Http.BaseAddress = new Uri(_developBaseAddress);
#endif
            return base.OnInitAsync();
        }
    }
}
