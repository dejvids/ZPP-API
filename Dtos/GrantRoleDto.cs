using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Dtos
{
    public class GrantRoleDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int CompanyId { get; set; }
    }
}
