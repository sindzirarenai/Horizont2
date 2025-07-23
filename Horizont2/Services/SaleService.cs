using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Horizont.Connection;
using Horizont.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Horizont.Services
{
    public class SaleService : ISaleService
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

        public List<Assortment> GetFrequentlyAssortment(ApplicationContext context)
        {
            var sales = context.Sales.ToList()
                .Select(x => new {SaleDocId = x.SaleDocumentId, AssortmentId = x.AssortmentId})
                .Distinct()
                .GroupBy(h => h.AssortmentId)
                .Select(t => new {key = t.Key, weight = t.Count()})
                .ToList();

            var result = sales.OrderByDescending(x => x.weight).Take(10).Select(y=>y.key).ToList();
            var assortments = result.Select(x => context.Assortments.FirstOrDefault(d => x == d.Id)).ToList();
            assortments.ForEach(x => x.Sales = new List<Sale>());
            return
                assortments;
        }

        public List<Contrpartner> GetContrpartnersByDivision(ApplicationContext context, String division)
        {
            var saleDocs = context.SaleDocuments
                .Where(x => x.Division.ToLower().Contains(division.ToLower()))
                .Select(l => l.ContrpartnerId).Distinct().Take(200).ToList();

            return context.Contrpartners.Where(j => saleDocs.Contains(j.Id))
                .ToList();
               
        }

        public List<Assortment> GetFrequentlyAssortmentByContrpartner(ApplicationContext context, long id)
        {
            var sales = context.Sales.ToList()
                .Select(x => new { SaleDocId = x.SaleDocumentId, AssortmentId = x.AssortmentId })
                .Distinct()
                .GroupBy(h => h.AssortmentId)
                .Select(t => new { key = t.Key, weight = t.Count() })
                .ToList();

            var result = sales.OrderByDescending(x => x.weight).Take(10).Select(y => y.key).ToList();
            var assortments = result.Select(x => context.Assortments.FirstOrDefault(d => x == d.Id)).ToList();
            assortments.ForEach(x => x.Sales = new List<Sale>());
            return assortments;
        }

        public List<Assortment> GetAprioriAssortment(long id, ApplicationContext context)
        {
            var contrpartnerDocIds = context.SaleDocuments.Where(x => x.ContrpartnerId == id).Select(x => x.Id).ToList();
            var saleIds = context.Sales.Where(x => contrpartnerDocIds.Contains(x.SaleDocumentId.GetValueOrDefault()))
                .Select(y => y.AssortmentId).Distinct().ToList();

            var saleDocId = context.Sales.Where(x => saleIds.Contains(x.AssortmentId.GetValueOrDefault()))
                .Select(x => x.SaleDocumentId).ToList();
            var sales = context.Sales.Where(x => saleDocId.Contains(x.SaleDocumentId)).ToList();
            var list = sales
                .Select(t => new
                {
                    id = t.SaleDocumentId, assortment = t.AssortmentId
                })
                .GroupBy(k => k.id).Select(x => new
                {
                    id = x.Key, set = x.Select(l => l.assortment).ToList()
                }).ToList();

            var dictionary = list
                .ToDictionary(y => y.id, x => x.set.OrderBy(z => z).ToList());
            
            var uniqueAssortments =
                dictionary.SelectMany(y => y.Value).Distinct().ToList();

            uniqueAssortments.RemoveAll(x => saleIds.Contains(x.GetValueOrDefault()));

            var supportSet = new Dictionary<Assortment, long>();

            foreach (var assortment in uniqueAssortments)
            {
                supportSet.Add(context.Assortments.FirstOrDefault(x => x.Id == assortment),
                    dictionary.Count(j => j.Value.Contains(assortment)));
            }

           var result = supportSet.OrderByDescending(o => o.Value)
               .Where(o => o.Value > 2)
               .Select(l => l.Key).Take(10)
               .ToList();

           result.ForEach(x=>x.Sales.Clear());

           return result;
        }

    }
}
