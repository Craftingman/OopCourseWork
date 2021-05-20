using System.Collections.Generic;
using Core;

namespace DAL.Abstractions
{
    public interface IRelativeDalService
    {
        void Add(Relative caste);

        void Delete(int id);
        
        Relative Get(int id);

        IEnumerable<Relative> GetAll();
    }
}