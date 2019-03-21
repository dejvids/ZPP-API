using ZPP_Blazor.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System.Linq;

namespace ZPP_Blazor.Components.Home
{
    public class HomeComponent: BaseComponent
    {
        public List<Lecture> Lectures { get; set; } 
        public List<Lecture> PromotingLectures { get; set; }

        protected override void OnInit()
        {
            //var response = Http.GetAsync("/api/lectures").Result;
            //if(response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    Lectures = Json.Deserialize<List<Lecture>>(response.Content.ReadAsStringAsync().Result);
            //    if(Lectures.Count >= 3)
            //    {
            //        PromotingLectures = Lectures.Take(3).ToList();
            //    }
            //}
        }
    }
}