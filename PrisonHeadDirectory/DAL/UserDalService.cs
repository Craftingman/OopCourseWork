using System.Linq;
using System.Xml.Linq;
using Core;
using DAL.Abstractions;

namespace DAL
{
    public class UserDalService : IUserDalService
    {
        private readonly XElement _database;

        public UserDalService(XElement xDocument)
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
                .FirstOrDefault(e => e.Attribute("id")?.Value == xUser.Element("roleId")?.Value);

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
                    Id = int.Parse(xRole?.Attribute("id")?.Value ?? "-1"),
                    Name = xRole?.Element("name")?.Value
                }
            };
        }

        public void CreateUser(User user, string roleName)
        {
            int.TryParse(_database
                .Element("roles")
                ?.Elements("role")
                .FirstOrDefault(e => e.Element("name")?.Value == roleName)
                ?.Element("id")
                ?.Value, out int roleId);

            int id = 0;
            
            if (!_database.Element("users").IsEmpty)
            {
                int.TryParse(_database
                    .Element("users")
                    ?.Elements("user")
                    .LastOrDefault()
                    ?.Element("id")
                    ?.Value, out id);
                id++;
            }
            
            XElement xUser = new XElement("user", 
                new XElement("name", user.Name),
                new XElement("surname", user.Name),
                new XElement("middleName", user.Name),
                new XElement("email", user.Name),
                new XElement("name", user.Password),
                new XElement("roleId", roleId),
                new XAttribute("id", id));
        }

        public bool UserExists(string email)
        {
            return _database.Element("users")
                ?.Elements("user")
                .FirstOrDefault(e => e.Element("email")?.Value == email) is not null;
        }
    }
}