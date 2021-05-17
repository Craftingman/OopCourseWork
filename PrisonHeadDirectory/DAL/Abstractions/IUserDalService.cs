using System.Collections;
using System.Collections.Generic;
using Core;

namespace DAL.Abstractions
{
    public interface IUserDalService
    {
        User GetUserForAuth(string email, string password);

        User GetUser(int id);

        IEnumerable<Role> GetRoles();

        IEnumerable<User> GetUsers(string searchStr = "");

        void CreateUser(User user, string roleName);

        void UpdateUser(User user, string roleName);

        void DeleteUser(int id);

        bool UserExists(string email);
    }
}