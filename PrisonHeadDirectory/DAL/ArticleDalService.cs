using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core;
using DAL.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class ArticleDalService : IArticleDalService
    {
        private readonly XElement _database;

        private readonly IConfiguration _configuration;

        public ArticleDalService(XElement xDocument, IConfiguration configuration)
        {
            _database = xDocument;
            _configuration = configuration;
        }
        
        public void Add(Article article)
        {
            int id = int.Parse(_database.Element("articles")?.Attribute("lastId").Value ?? string.Empty) + 1;
            article.Id = id;

            XElement xArticle = new XElement("article", 
                new XElement("name", article.Name),
                new XElement("number", article.Number),
                new XAttribute("id", id));
            
            _database.Element("articles")?.Add(xArticle);
            _database.Element("articles").Attribute("lastId").Value = id.ToString();
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public void Delete(int id)
        {
            XElement xArticle = _database.Element("articles")
                ?.Elements("article")
                .FirstOrDefault(el => el.Attribute("id")?.Value == id.ToString());
            
            xArticle?.Remove();
            
            _database.Save(_configuration.GetConnectionString("Database"));
        }

        public Article Get(int id)
        {
            XElement xArticle = _database.Element("articles")
                ?.Elements("article")
                .FirstOrDefault(el => el.Attribute("id")?.Value == id.ToString());

            if (xArticle == null)
            {
                return null;
            }

            Article article = new Article()
            {
                Id = int.Parse(xArticle?.Attribute("id")?.Value ?? string.Empty),
                Name = xArticle?.Element("name")?.Value,
                Number = xArticle?.Element("number")?.Value
            };

            return article;
        }

        public IEnumerable<Article> GetAll()
        {
            List<Article> articles = new List<Article>();

            foreach (var xArticle in _database.Element("articles")?.Elements("article"))
            {
                articles.Add(new Article()
                {
                    Name = xArticle?.Element("name")?.Value,
                    Number = xArticle?.Element("number")?.Value,
                    Id = int.Parse(xArticle?.Attribute("id")?.Value ?? string.Empty)
                });
            }

            return articles;
        }
    }
}