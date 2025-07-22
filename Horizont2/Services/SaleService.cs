using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Horizont.Connection;
using Horizont.Models;

namespace Horizont.Services
{
    public class SaleService:ISaleService
    {
       private IBaseRepository<Sale> Sales { get; set; }
        private IBaseRepository<SaleDocument> SaleDocuments { get; set; }
        private IBaseRepository<Contrpartner> Contrpartners { get; set; }
        private IBaseRepository<Assortment> Assortments { get; set; }



        public List<Contrpartner> GetContrpartners()
        {
            return Contrpartners.GetAll();
        }

  

        public List<SaleDocument> GetSaleDocumentsByContrpartner(Contrpartner contrpartner)
        {
            return contrpartner.SaleDocuments.ToList();
        }

        /*public List<Assortment> GetAprioriAssortment(List<long> ids)
        {
            var dictionary = SaleDocuments.Linq()
                .Where(x => x.GetAssortments().Count(y => ids.Contains(y.Id)) == ids.Count)
                .ToDictionary(y => y, x => x.Sales.Select(z => z.Assortment).OrderBy(z => z.Id).ToList());
            var uniqueAssortments =
                dictionary.SelectMany(x => x.Value).Distinct().Where(x => !ids.Contains(x.Id)).ToList();

            var supportDictionary = new Dictionary<Assortment, int>();
            foreach (var uniqueAssortment in uniqueAssortments)
            {
                supportDictionary.Add(uniqueAssortment, dictionary.Where(x=>x.Value.Contains()));   
            }
        }*/
    }
}
