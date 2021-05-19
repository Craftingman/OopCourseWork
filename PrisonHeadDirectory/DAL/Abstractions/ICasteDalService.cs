using System.Collections;
using System.Collections.Generic;
using Core;

namespace DAL.Abstractions
{
    public interface ICasteDalService
    {
        void Add(Caste caste);

        void Delete(int id);
        
        Caste Get(int id);

        IEnumerable<Caste> GetAll();
    }
}