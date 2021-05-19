using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Core;
using DAL.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class UserDalService : IUserDalService
    {
        private readonly XElement _database;

        private readonly IConfiguration _configuration;

        public UserDalService(XElement xDocument, IConfiguration configuration)
        {
            _database = xDocument;
            _configuration = configuration;
        }

        public User GetUserForAuth(string email, string password)
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
                Id = int.Parse(xUser?.Attribute("id")?.Value ?? "-1"),
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

        public User GetUser(int id)
        {
            XElement xUser = _database.Element("users")
                ?.Elements("user")
                .FirstOrDefault(e => e.Attribute("id")?.Value == id.ToString());

            if (xUser is null)
            {
                return null;
            }

            XElement xRole = _database.Element("roles")
                ?.Elements("role")
                .FirstOrDefault(e => e.Attribute("id")?.Value == xUser.Element("roleId")?.Value);

            return new User()
            {
                Id = int.Parse(xUser?.Attribute("id")?.Value ?? "-1"),
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

        public IEnumerable<Role> GetRoles()
        {
            List<Role> roles = new List<Role>();
            
            foreach (var xRole in _database.Element("roles")?.Elements("role"))
            {
                roles.Add(new Role()
                {
                    Id = int.Parse(xRole?.Attribute("id")?.Value ?? "-1"),
                    Name = xRole?.Element("name")?.Value
                });
            }

            return roles;
        }

        public void CreateUser(User user, string roleName)
        {
            int.TryParse(_database
                .Element("roles")
                ?.Elements("role")
                .FirstOrDefault(e => e.Element("name")?.Value == roleName)
                ?.Attribute("id")
                ?.Value, out int roleId);

            int id = int.Parse(_database.Element("users")?.Attribute("lastId").Value ?? string.Empty) + 1;

            XElement xUser = new XElement("user", 
                new XElement("name", user.Name),
                new XElement("surname", user.Surname),
                new XElement("middleName", user.MiddleName),
                new XElement("email", user.Email.ToLower()),
                new XElement("password", user.Password),
                new XElement("roleId", roleId),
                new XAttribute("id", id));
            
            _database.Element("users")?.Add(xUser);
            _database.Element("users").Attribute("lastId").Value = id.ToString();
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public void UpdateUser(User user, string roleName)
        {
            XElement xUser = _database.Element("users")
                ?.Elements("user")
                .FirstOrDefault(e => e.Attribute("id")?.Value == user.Id.ToString());

            if (xUser == null)
            {
                return;
            }

            int.TryParse(_database
                .Element("roles")
                ?.Elements("role")
                .FirstOrDefault(e => e.Element("name")?.Value == roleName)
                ?.Attribute("id")
                ?.Value, out int roleId);

            xUser.Element("name").Value = user.Name;
            xUser.Element("surname").Value = user.Surname;
            xUser.Element("middleName").Value = user.MiddleName;
            xUser.Element("email").Value = user.Email;
            xUser.Element("roleId").Value = roleId.ToString();
            
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public void DeleteUser(int id)
        {
            XElement xUser = _database.Element("users")
                ?.Elements("user")
                .FirstOrDefault(e => e.Attribute("id")?.Value == id.ToString());

            xUser?.Remove();
            
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public IEnumerable<User> GetUsers(string searchStr)
        {
            List<User> users = new List<User>();
            string normalizedSearchString = searchStr.ToLower();

            foreach (var xUser in _database.Element("users")?.Elements("user"))
            {
                XElement xRole = _database.Element("roles")
                    ?.Elements("role")
                    .FirstOrDefault(e => e.Attribute("id")?.Value == xUser.Element("roleId")?.Value);

                users.Add(new User()
                {
                    Id = int.Parse(xUser?.Attribute("id")?.Value ?? "-1"),
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
                });
            }

            return users.Where(u => u.Name.ToLower().Contains(normalizedSearchString)
                                    || u.Surname.ToLower().Contains(normalizedSearchString)
                                    || u.MiddleName.ToLower().Contains(normalizedSearchString)
                                    || u.Email.Contains(normalizedSearchString));
        }

        public bool UserExists(string email)
        {
            return _database.Element("users")
                ?.Elements("user")
                .FirstOrDefault(e => e.Element("email")?.Value == email) is not null;
        }
    }
}