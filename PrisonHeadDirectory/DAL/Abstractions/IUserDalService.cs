using Core;

namespace DAL.Abstractions
{
    public interface IUserDalService
    {
        User GetUser(string email, string password);

        void CreateUser(User user, string roleName);

        bool UserExists(string email);
    }
}