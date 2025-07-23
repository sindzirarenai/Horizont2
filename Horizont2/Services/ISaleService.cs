using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizont.Connection;
using Horizont.Models;

namespace Horizont.Services
{
    public interface ISaleService
    {
        List<Assortment> GetAssortmentApriori(long id, ApplicationContext context);
        List<Assortment> GetFrequentlyAssortment(ApplicationContext context);
        List<Contrpartner> GetContrpartnersByDivision(ApplicationContext context, string division);
        List<Assortment> GetFrequentlyAssortmentByContrpartner(ApplicationContext context, long id);
    }
}
