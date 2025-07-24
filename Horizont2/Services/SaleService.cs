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

        private List<List<long>> GetSetAssortmentsFromDocuments (List<long> saleDocumentIds, ApplicationContext context)
        {
            return context.Sales
                .Where(x => saleDocumentIds.Contains(x.SaleDocumentId.GetValueOrDefault()))
                .ToList()
                .GroupBy(x => x.SaleDocumentId)
                .Select(z => z.Select(t => t.AssortmentId.GetValueOrDefault()).ToList().OrderBy(x => x).ToList())
                .ToList();
        }

        public List<Assortment> GetAssortmentApriori(long id, ApplicationContext context)
        {
            var levelCalculate = 10;

            //продажи контрагента
            var contrpartnerDocIds =
                context.SaleDocuments.Where(x => x.ContrpartnerId == id).Select(x => x.Id).ToList();

            if (!contrpartnerDocIds.Any()) return new List<Assortment>();
            
            //вычисляем популярный набор у контрагента (не более ограничения)
            var assortmentFrequentlyIds = GetSetAssortmentsFromDocuments(contrpartnerDocIds, context)
                .Where(x => x.Count <= levelCalculate).ToList()
                .GroupBy(x => x)
                .Select(z => new {Assortment = z.Key, Cnt = z.Count()})
                .OrderByDescending(x => x.Cnt).FirstOrDefault()?.Assortment;

           if (assortmentFrequentlyIds == null) return new List<Assortment>();

           //Ищем подходящие документы других контрагентов с подходящим сортаментом
           var saleDocId = context.Sales.Where(x =>
                   !contrpartnerDocIds.Contains(x.SaleDocumentId.GetValueOrDefault())
                   && assortmentFrequentlyIds.Contains(x.AssortmentId.GetValueOrDefault()))
               .Select(x => x.SaleDocumentId.GetValueOrDefault()).ToList();

           //Достаем наборы сортамента, содержащие набор контрагента
           var assortmentForSupport = GetSetAssortmentsFromDocuments(saleDocId, context)
               .Where(x => x.Count(t => assortmentFrequentlyIds.Contains(t)) == assortmentFrequentlyIds.Count);

           //Берем уникальные сортаменты из наборов
           var uniqueAssortments = assortmentForSupport.SelectMany(y => y).Distinct().ToList();
           uniqueAssortments.RemoveAll(x => assortmentFrequentlyIds.Contains(x));

           var supportSet = new Dictionary<Assortment, long>();

            foreach (var assortment in uniqueAssortments)
            {
                supportSet.Add(context.Assortments.FirstOrDefault(x => x.Id == assortment),
                    assortmentForSupport.Count(j => j.Contains(assortment)));
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
