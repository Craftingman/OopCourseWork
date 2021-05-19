using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core;
using DAL.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class PrisonerDalService : IPrisonerDalService
    {
        private readonly XElement _database;

        private readonly IConfiguration _configuration;

        public PrisonerDalService(XElement xDocument, IConfiguration configuration)
        {
            _database = xDocument;
            _configuration = configuration;
        }
        public IEnumerable<Prisoner> GetPrisoners(string searchStr = "")
        {
            List<Prisoner> prisoners = new List<Prisoner>();
            string normalizedSearchString = searchStr.ToLower();

            foreach (var xPrisoner in _database.Element("prisoners")?.Elements("prisoner"))
            {
                int prisonerId = int.Parse(xPrisoner?.Attribute("id")?.Value ?? "-1");
                List<Relative> relatives = GetRelatives(prisonerId);
                
                prisoners.Add(new Prisoner()
                {
                    Id = int.Parse(xPrisoner?.Attribute("id")?.Value ?? "-1"),
                    Name = xPrisoner.Element("name")?.Value,
                    Surname = xPrisoner.Element("surname")?.Value,
                    MiddleName = xPrisoner.Element("middleName")?.Value,
                    ReleaseDate = DateTime.Parse(xPrisoner.Element("releaseDate")?.Value ?? string.Empty),
                    ArrestDate = DateTime.Parse(xPrisoner.Element("arrestDate")?.Value ?? string.Empty),
                    Notes = xPrisoner.Element("notes")?.Value,
                    Cell = xPrisoner.Element("cell")?.Value,
                    ImgPath = xPrisoner.Element("imgPath")?.Value,
                    Relatives = relatives,
                    ArticleId = int.Parse(xPrisoner.Element("articleId")?.Value ?? string.Empty),
                    CasteId = int.Parse(xPrisoner.Element("casteId")?.Value ?? string.Empty)
                });
            }

            return prisoners.Where(p => p.Name.ToLower().Contains(normalizedSearchString)
                                    || p.Surname.ToLower().Contains(normalizedSearchString)
                                    || p.MiddleName.ToLower().Contains(normalizedSearchString));
        }

        public void Create(Prisoner prisoner)
        {
            int id = int.Parse(_database.Element("prisoners")?.Attribute("lastId").Value ?? string.Empty) + 1;
            prisoner.Id = id;

            XElement xPrisoner = new XElement("prisoner", 
                new XElement("name", prisoner.Name),
                new XElement("surname", prisoner.Surname),
                new XElement("middleName", prisoner.MiddleName),
                new XElement("releaseDate", prisoner.ReleaseDate),
                new XElement("arrestDate", prisoner.ArrestDate),
                new XElement("notes", prisoner.Notes),
                new XElement("cell", prisoner.Cell),
                new XElement("imgPath", prisoner.ImgPath),
                new XElement("casteId", prisoner.CasteId),
                new XElement("articleId", prisoner.ArticleId),
                new XAttribute("id", id));
            
            _database.Element("prisoners")?.Add(xPrisoner);
            _database.Element("prisoners").Attribute("lastId").Value = id.ToString();
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        private List<Relative> GetRelatives(int prisonerId)
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