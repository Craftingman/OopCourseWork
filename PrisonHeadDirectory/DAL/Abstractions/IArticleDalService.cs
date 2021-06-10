using System.Collections.Generic;
using Core;

namespace DAL.Abstractions
{
    public interface IArticleDalService
    {
        void Add(Article article);
        
        void Delete(int id);
        
        Article Get(int id);
        
        IEnumerable<Article> GetAll();
    }
}