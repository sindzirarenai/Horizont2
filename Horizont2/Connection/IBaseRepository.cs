using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizont.Models;

namespace Horizont.Connection
{
    public interface IBaseRepository<TDbModel> where TDbModel : BaseModel
    {
         List<TDbModel> GetAll();
         TDbModel Get(long id);
         TDbModel Create(TDbModel model);
         TDbModel Update(TDbModel model);
         void Delete(long id);

         List<TDbModel> GetAllByCriteria(Func<TDbModel, bool> predicate);

         IEnumerable<TDbModel> Linq();
    }

}
