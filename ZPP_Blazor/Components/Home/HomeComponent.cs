using ZPP_Blazor.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace ZPP_Blazor.Components.Home
{
    public class HomeComponent : BaseComponent
    {
        public List<Lecture> Lectures { get; set; }
        public List<Lecture> PromotingLectures { get; set; }

        public HomeComponent()
        { }

        protected override async Task OnInitAsync()
        {
            PromotingLectures = new List<Lecture> { new Lecture(), new Lecture(), new Lecture() };
            await base.OnInitAsync();
            Console.WriteLine("OnInit");
            if (Http == null)
            {
                Console.WriteLine("Http is null");
            }
            Console.WriteLine(AppCtx.BaseAddress);
            var response = await Http.GetAsync("/api/lectures");
            if (response == null)
            {
                Console.WriteLine("Result is null");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Lectures = Json.Deserialize<List<Lecture>>(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine("Pobranych wyk³adów " + Lectures.Count);
                if (Lectures.Count >= 3)
                {
                    PromotingLectures = Lectures.Take(3).ToList();
                }
            }
        }
    }
}