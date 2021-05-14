using System.Linq;
using System.Xml.Linq;
using Core;
using DAL.Abstractions;

namespace DAL
{
    public class UserDalService : IUserDalService
    {
        private readonly XDocument _database;

        public UserDalService(XDocument xDocument)
        {
            _database = xDocument;
        }

        public User GetUser(string email, string password)
        {
            XElement xUser = _database.Element("users")
                ?.Elements("user")
                .FirstOrDefault(e => e.Element("email")?.Value == email
                                     && e.Element("password")?.Value == password);

            if (xUser is null)
            {
                return null;
            }

            XElement xRole = _database.Element("roles")
                ?.Elements("role")
                .FirstOrDefault(e => e.Element("id")?.Value == xUser.Element("roleId")?.Value);

            return new User()
            {
                Id = int.Parse(xUser?.Element("id")?.Value ?? "-1"),
                Email = xUser.Element("email")?.Value,
                Password = string.Empty,
                Name = xUser.Element("name")?.Value,
                Surname = xUser.Element("surname")?.Value,
                MiddleName = xUser.Element("middleName")?.Value,
                Role = new Role()
                {
                    Id = int.Parse(xRole?.Element("id")?.Value ?? "-1"),
                    Name = xRole?.Element("name")?.Value
                }
            };
        }

        public void CreateUser(User user, string roleName)
        {
            throw new System.NotImplementedException();
        }

        public bool UserExists(string email)
        {
            throw new System.NotImplementedException();
        }
    }
}