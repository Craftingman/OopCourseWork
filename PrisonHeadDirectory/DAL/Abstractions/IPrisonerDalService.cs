using System.Collections;
using System.Collections.Generic;
using Core;

namespace DAL.Abstractions
{
    public interface IPrisonerDalService
    {
        IEnumerable<Prisoner> GetPrisoners(string searchStr = "");

        void Create(Prisoner prisoner);

        void Update(Prisoner prisoner);
        
        void Delete(int id);

        Prisoner Get(int id);
    }
}