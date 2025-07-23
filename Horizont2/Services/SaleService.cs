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
            var regionDoc = context.SaleDocuments.FirstOrDefault(x => x.ContrpartnerId==id).Division;
            var saleDocIds = context.SaleDocuments.Where(x => x.ContrpartnerId != id && x.Division == regionDoc)
                .Select(x => x.Id).ToList();

            var sales = context.Sales.Where(x => saleDocIds.Contains(x.SaleDocumentId.GetValueOrDefault())).ToList()
                .Select(x => new {SaleDocId = x.SaleDocumentId, AssortmentId = x.AssortmentId})
                .Distinct()
                .GroupBy(h => h.AssortmentId)
                .Select(t => new {key = t.Key, weight = t.Count()})
                .ToList();

            var result = sales.OrderByDescending(x => x.weight).Take(10).Select(y => y.key).ToList();
            var assortments = result.Select(x => context.Assortments.FirstOrDefault(d => x == d.Id)).ToList();
            assortments.ForEach(x => x.Sales = new List<Sale>());
            return assortments;
        }

        public List<Assortment> GetAssortmentApriori(long id, ApplicationContext context)
        {
            var contrpartnerDocIds = context.SaleDocuments.Where(x => x.ContrpartnerId == id).Select(x => x.Id).ToList();
            var assortmentIds = context.Sales.Where(x => contrpartnerDocIds.Contains(x.SaleDocumentId.GetValueOrDefault()))
                .Select(y => y.AssortmentId).Distinct().ToList();

            var saleDocId = context.Sales.Where(x =>
                    !contrpartnerDocIds.Contains(x.SaleDocumentId.GetValueOrDefault()) &&
                    assortmentIds.Contains(x.AssortmentId.GetValueOrDefault()))
                .Select(x => x.SaleDocumentId).ToList();

            var sales = context.Sales.Where(x => saleDocId.Contains(x.SaleDocumentId)).ToList();

            var list = sales
                .GroupBy(k => k.SaleDocumentId).Select(x => new
                {
                    id = x.Key, set = x.Select(l => l.AssortmentId).ToList()
                }).ToList();

            var dictionary = list
                .ToDictionary(y => y.id, x => x.set.OrderBy(z => z).ToList());
            
            var uniqueAssortments = dictionary.SelectMany(y => y.Value).Distinct().ToList();
            uniqueAssortments.RemoveAll(x => assortmentIds.Contains(x.GetValueOrDefault()));

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
