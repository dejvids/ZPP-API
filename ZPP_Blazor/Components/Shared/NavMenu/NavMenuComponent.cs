using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP_Blazor.Components.Shared.NavMenu
{
    public class NavMenuComponent : BlazorComponent
    {
        public bool IsExpanded { get; set; }
        public bool IsSigned { get; set; }

        public void Toggle()
        {
            this.IsExpanded = !this.IsExpanded;
        }
    }
}
