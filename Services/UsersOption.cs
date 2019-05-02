namespace ZPP.Server.Services
{
    public class UsersOption
    {
        public int PerPage { get; set; }

        public UsersOption(int usersPerPage)
        {
            this.PerPage = usersPerPage;
        }

    }
}
