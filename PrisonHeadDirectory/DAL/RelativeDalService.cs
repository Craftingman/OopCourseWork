using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core;
using DAL.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class RelativeDalService : IRelativeDalService
    {
        private readonly XElement _database;

        private readonly IConfiguration _configuration;

        public RelativeDalService(XElement xDocument, IConfiguration configuration)
        {
            _database = xDocument;
            _configuration = configuration;
        }
        public void Add(Relative relative)
        {
            int id = int.Parse(_database.Element("relatives")?.Attribute("lastId").Value ?? string.Empty) + 1;
            relative.Id = id;

            XElement xRelative = new XElement("relative", 
                new XElement("name", relative.Name),
                new XElement("surname", relative.Surname),
                new XElement("middleName", relative.MiddleName),
                new XElement("address", relative.Address),
                new XElement("phoneNumber", relative.PhoneNumber),
                new XElement("relativeRole", relative.RelativeRole),
                new XAttribute("prisonerId", relative.PrisonerId),
                new XAttribute("id", id));
            
            _database.Element("relatives")?.Add(xRelative);
            _database.Element("relatives").Attribute("lastId").Value = id.ToString();
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public void Delete(int id)
        {
            XElement xRelative = _database.Element("relatives")
                ?.Elements("relative")
                .FirstOrDefault(el => el.Attribute("id")?.Value == id.ToString());

            xRelative?.Remove();
            
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public Relative Get(int id)
        {
            XElement xRelative = _database.Element("relatives")
                ?.Elements("relative")
                .FirstOrDefault(el => el.Attribute("id")?.Value == id.ToString());

            Relative relative = new Relative()
            {
                Address = xRelative.Element("address")?.Value,
                PhoneNumber = xRelative.Element("phoneNumber")?.Value,
                RelativeRole = xRelative.Element("relativeRole")?.Value,
                PrisonerId = int.Parse(xRelative.Attribute("prisonerId")?.Value ?? String.Empty),
                Name = xRelative.Element("name")?.Value,
                Surname = xRelative.Element("surname")?.Value,
                MiddleName = xRelative.Element("middleName")?.Value,
            };

            return relative;
        }

        public IEnumerable<Relative> GetAll(int prisonerId)
        {
            List<Relative> relatives = new List<Relative>();

            IEnumerable<XElement> xRelatives = _database.Element("relatives")
                ?.Elements("relative")
                .Where(el => el.Attribute("prisonerId")?.Value == prisonerId.ToString());

            foreach (var xRelative in xRelatives)
            {
                relatives.Add(new Relative()
                {
                    Address = xRelative.Element("address")?.Value,
                    PhoneNumber = xRelative.Element("phoneNumber")?.Value,
                    RelativeRole = xRelative.Element("relativeRole")?.Value,
                    PrisonerId = prisonerId,
                    Name = xRelative.Element("name")?.Value,
                    Surname = xRelative.Element("surname")?.Value,
                    MiddleName = xRelative.Element("middleName")?.Value,
                });
            }

            return relatives;
        }
    }
}