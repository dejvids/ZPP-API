using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Dtos
{
    public class CompanyDto
    {
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }
        public string Url { get; set; }
        public List<UserDto> Lecturers { get; set; }
    }
}
