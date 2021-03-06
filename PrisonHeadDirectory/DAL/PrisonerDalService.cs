using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core;
using DAL.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

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
                    Relatives = relatives,
                    Articles = GetArticles(int.Parse(xPrisoner?.Attribute("id")?.Value ?? "-1")),
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
            
            string articlesStr = string.Join(",", prisoner.Articles
                .Select(a => a.Id.ToString()));

            XElement xPrisoner = new XElement("prisoner", 
                new XElement("name", prisoner.Name),
                new XElement("surname", prisoner.Surname),
                new XElement("middleName", prisoner.MiddleName),
                new XElement("releaseDate", prisoner.ReleaseDate),
                new XElement("arrestDate", prisoner.ArrestDate),
                new XElement("notes", prisoner.Notes),
                new XElement("cell", prisoner.Cell),
                new XElement("casteId", prisoner.CasteId),
                new XElement("articles", articlesStr),
                new XAttribute("id", id));
            
            _database.Element("prisoners")?.Add(xPrisoner);
            _database.Element("prisoners").Attribute("lastId").Value = id.ToString();
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public void Update(Prisoner prisoner)
        {
            int id = prisoner.Id;

            XElement xPrisoner = _database.Element("prisoners")
                .Elements("prisoner")
                .FirstOrDefault(p => p.Attribute("id")?.Value == id.ToString());

            if (xPrisoner is null)
            {
                return;
            }
            
            string articlesStr = string.Join(",", prisoner.Articles
                .Select(a => a.Id.ToString()));

            xPrisoner.Element("name").Value = prisoner.Name;
            xPrisoner.Element("surname").Value = prisoner.Surname;
            xPrisoner.Element("middleName").Value = prisoner.MiddleName;
            xPrisoner.Element("releaseDate").Value = prisoner.ReleaseDate.ToString();
            xPrisoner.Element("arrestDate").Value = prisoner.ArrestDate.ToString();
            xPrisoner.Element("notes").Value = prisoner.Notes ?? String.Empty;
            xPrisoner.Element("cell").Value = prisoner.Cell;
            xPrisoner.Element("casteId").Value = prisoner.CasteId.ToString();
            xPrisoner.Element("articles").Value = articlesStr;
            
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public void Delete(int id)
        {
            XElement xPrisoner = _database.Element("prisoners")
                .Elements("prisoner")
                .FirstOrDefault(p => p.Attribute("id")?.Value == id.ToString());

            if (xPrisoner is null)
            {
                return;
            }
            
            xPrisoner.Remove();
            
            List<XElement> xRelatives = _database.Element("relatives")
                ?.Elements("relative")
                .Where(el => el.Attribute("prisonerId")?.Value == id.ToString()).ToList();

            if (xRelatives is null)
            {
                return;
            }

            foreach (var xRelative in xRelatives)
            {
                xRelative.Remove();
            }
            
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public Prisoner Get(int id)
        {
            XElement xPrisoner = _database.Element("prisoners")
                .Elements("prisoner")
                .FirstOrDefault(p => p.Attribute("id")?.Value == id.ToString());

            Prisoner prisoner = new Prisoner()
            {
                Name = xPrisoner.Element("name").Value,
                Surname = xPrisoner.Element("surname").Value,
                MiddleName = xPrisoner.Element("middleName").Value,
                ReleaseDate = DateTime.Parse(xPrisoner.Element("releaseDate").Value),
                ArrestDate = DateTime.Parse(xPrisoner.Element("arrestDate").Value),
                Notes = xPrisoner.Element("notes").Value,
                Cell = xPrisoner.Element("cell").Value,
                CasteId = Int32.Parse(xPrisoner.Element("casteId").Value ?? String.Empty),
                Relatives = GetRelatives(id),
                Articles = GetArticles(id),
                Id = Int32.Parse(xPrisoner.Attribute("id").Value ?? String.Empty)
            };

            return prisoner;
        }

        private List<Article> GetArticles(int prisonerId)
        {
            XElement xPrisoner = _database.Element("prisoners")
                .Elements("prisoner")
                .FirstOrDefault(p => p.Attribute("id")?.Value == prisonerId.ToString());

            List<Article> articles = new List<Article>();

            foreach (var articleIdStr in xPrisoner.Element("articles").Value.Split(","))
            {
                if (string.IsNullOrEmpty(articleIdStr))
                {
                    continue;
                }

                XElement xArticle = _database.Element("articles")
                    .Elements("article")
                    .FirstOrDefault(p => p.Attribute("id")?.Value == articleIdStr);

                if (xArticle is null)
                {
                    continue;
                }

                articles.Add( new Article()
                {
                    Id = Int32.Parse(xArticle.Attribute("id").Value ?? String.Empty),
                    Name = xArticle.Element("name")?.Value,
                    Number = xArticle.Element("number")?.Value
                });
            }

            return articles;
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
                    Id = int.Parse(xRelative.Attribute("id")?.Value ?? string.Empty)
                });
            }

            return relatives;
        }
    }
}