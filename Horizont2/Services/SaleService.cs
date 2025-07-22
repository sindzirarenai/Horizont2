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
            var sales = context.Sales.Where(x => ids.Contains(x.AssortmentId.GetValueOrDefault())).ToList();
            var list =    sales.Select(t => new
                {
                    id = t.SaleDocumentId, assortment = context.Assortments.FirstOrDefault(u => u.Id == t.AssortmentId)
                })
                .GroupBy(k => k.id).Select(x => new
                {
                    id = x.Key, set = x.Select(l=>l.assortment).ToList()
                }).ToList();

            var dictionary = list
                .ToDictionary(y => y.id, x => x.set.OrderBy(z => z.Id).ToList());
            
            var uniqueAssortments =
                dictionary.SelectMany(y => y.Value).Distinct().Where(l => !ids.Contains(l.Id)).ToList();

            var supportSet = new Dictionary<Assortment, long>();

            foreach (var assortment in uniqueAssortments)
            {
                supportSet.Add(assortment,  dictionary.Count(j => j.Value.Contains(assortment)));
            }

            return supportSet.OrderByDescending(o => o.Value).Where(o => o.Value > 2).Select(l => l.Key).Take(10)
                .ToList();
        }

    }
}
