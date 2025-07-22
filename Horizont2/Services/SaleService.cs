using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Horizont.Connection;
using Horizont.Models;
using Microsoft.AspNetCore.Mvc;

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

        public List<Assortment> GetAprioriAssortment(List<long> ids, ApplicationContext context)
        {
            var dictionary = context.SaleDocuments
                .Where(x => x.GetAssortments().Count(y => ids.Contains(y.Id)) == ids.Count)
                .ToDictionary(y => y.Id, x => x.Sales.Select(z => z.Assortment).OrderBy(z => z.Id).ToList());
            
            var uniqueAssortments =
                dictionary.SelectMany(y => y.Value).Distinct().Where(l => !ids.Contains(l.Id)).ToList();

            var supportSet = new Dictionary<Assortment, long>();

            foreach (var assortment in uniqueAssortments)
            {
                supportSet.Add(assortment,  dictionary.Count(j => j.Value.Contains(assortment)));
            }

            return supportSet.OrderByDescending(o => o.Value).Select(l => l.Key).Take(10).ToList();
        }

    }
}
