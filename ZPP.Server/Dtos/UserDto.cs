using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Entities;

namespace ZPP.Server.Dtos
{
    public class UserDto
    {
        public UserDto()
        {}
        public UserDto(User user)
        {
            Login = user.Login;
            Surname = user.Surname;
            Name = user.Name;
            Email = user.Email;
        }

        public string Login { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
