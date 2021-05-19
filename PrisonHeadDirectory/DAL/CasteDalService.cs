using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core;
using DAL.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class CasteDalService : ICasteDalService
    {
        private readonly XElement _database;

        private readonly IConfiguration _configuration;

        public CasteDalService(XElement xDocument, IConfiguration configuration)
        {
            _database = xDocument;
            _configuration = configuration;
        }
        
        public void Add(Caste caste)
        {
            int id = int.Parse(_database.Element("castes")?.Attribute("lastId").Value ?? string.Empty) + 1;
            caste.Id = id;

            XElement xCaste = new XElement("prisoner", 
                new XElement("name", caste.Name),
                new XAttribute("id", id));
            
            _database.Element("castes")?.Add(xCaste);
            _database.Element("castes").Attribute("lastId").Value = id.ToString();
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public Caste Get(int id)
        {
            XElement xCaste = _database.Element("castes")
                ?.Elements("caste")
                .FirstOrDefault(el => el.Attribute("id")?.Value == id.ToString());

            Caste caste = new Caste()
            {
                Name = xCaste?.Element("name")?.Value
            };

            return caste;
        }

        public IEnumerable<Caste> GetAll()
        {
            List<Caste> castes = new List<Caste>();

            foreach (var xCaste in _database.Element("castes")?.Elements("caste"))
            {
                castes.Add(new Caste()
                {
                    Name = xCaste?.Element("name")?.Value,
                    Id = int.Parse(xCaste?.Attribute("id")?.Value ?? string.Empty)
                });
            }

            return castes;
        }
    }
}