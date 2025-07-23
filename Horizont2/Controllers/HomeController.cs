using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Horizont.Connection;
using Horizont.Models;
using Horizont.Services;

namespace Horizont2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> _logger;
        private ApplicationContext Context { get; set; }
        private ISaleService Service { get; set; }

        public HomeController(ILogger<HomeController> logger, ApplicationContext dbContext, ISaleService service)
        {
            Context = dbContext;
            _logger = logger;
            Service = service;
        }

        [HttpGet("GetContrpartnerByName")]
        public List<JsonResult> GetContrpartnerByName(String name)
        {
            return Context.Contrpartners.Where(x => x.ContrpartnerName.ToLower().Contains(name.ToLower())).ToList()
                .ConvertAll(x => new JsonResult(x));
        }


        [HttpGet("GetContrpartnerByInn")]
        public List<JsonResult> GetContrpartnerByInn(long inn)
        {
            return Context.Contrpartners.Where(x => x.ContrpartnerInn.ToString().ToLower().Contains(inn.ToString().ToLower())).ToList()
                .ConvertAll(x => new JsonResult(x));
        }

        [HttpGet("GetContrpartnerByDivision")]
        public List<JsonResult> GetContrpartnerByDivision(String division)
        {
           var saleDocs=  Context.SaleDocuments
                .Where(x => x.Division.ToLower().Contains(division.ToLower()))
                .Select(l=>l.ContrpartnerId).Distinct().Take(200).ToList();

             return   Context.Contrpartners.Where(j => saleDocs.Contains(j.Id))
                .ToList()
                .ConvertAll(x => new JsonResult(x));
        }

        [HttpGet("GetContrpartnerByWarehouse")]
        public List<JsonResult> GetContrpartnerByWarehouse(String warehouse)
        {
            var saleDocs = Context.SaleDocuments
                .Where(x => x.Warehouse.ToLower().Contains(warehouse.ToLower()))
                .Select(l => l.ContrpartnerId).Distinct().Take(200).ToList();

            return Context.Contrpartners.Where(j => saleDocs.Contains(j.Id))
                .ToList()
                .ConvertAll(x => new JsonResult(x));
        }

        [HttpGet("GetAssortments")]
         public List<JsonResult> GetAssortments()
         {
             return Context.Assortments.Take(500).ToList().ConvertAll(x => new JsonResult(x));
         }

         [HttpGet("GetSaleDocumentsByContrpartner")]
         public List<JsonResult> GetSaleDocumentsByContrpartner(long id)
         {
             var listSales = Context.SaleDocuments
                 .Where(x => x.ContrpartnerId == id 
                             && Context.Sales.Any(u => u.SaleDocumentId == x.Id)).ToList();
             var assortment = listSales.Select(i => new
                     {SaleDocument = i, Assortments = Context.Sales.Where(s => s.SaleDocumentId == i.Id)})
                 .ToList().Select(o => new
                 {
                     document = o.SaleDocument,
                     assortments = Context.Assortments.Where(p => o.Assortments.Any(l => l.AssortmentId == p.Id))
                         .Distinct().ToList()
                 }).ToList();

             return assortment.ConvertAll(u => new JsonResult(u));
         }

        [HttpGet("GetTnsByContrpartner")]
         public JsonResult GetTnsByContrpartner(long id)
         {
             var documentIds = Context.SaleDocuments.Where(x => x.ContrpartnerId == id).Select(y=>y.Id).ToList();
             return new JsonResult(Context.Sales.Where(x => documentIds.Contains(x.SaleDocumentId.GetValueOrDefault()))
                 .Select(y => y.Tns).Sum());
         }

         [HttpGet("GetTnsByMonths")]
         public List<JsonResult> GetTnsByMonths(long id)
         {
             var monthTns = Context.SaleDocuments.Where(x => x.ContrpartnerId == id)
                 .Select(x => new
                 {
                     Month = x.DocumentDate.Value.Month,
                     SalesSum = Context.Sales.Where(t => t.SaleDocumentId == x.Id).Select(t=>t.Tns).Sum()
                 })
                 .GroupBy(t => t.Month)
                 .Select(y => new {Month = y.Key, Tns = y.Sum(r=>r.SalesSum)}).ToList();

             return monthTns.ConvertAll(x => new JsonResult(x));
         }

         [HttpGet("GetTnsBySuppliers")]
         public List<JsonResult> GetTnsBySuppliers(long id)
         {
            var supplierTns = Context.SaleDocuments.Where(x => x.ContrpartnerId == id)
                .Select(x => new
                {
                    Supplier = x.Supplier,
                    SalesSum = Context.Sales.Where(t => t.SaleDocumentId == x.Id).Select(t => t.Tns).Sum()
                })
                .GroupBy(t => t.Supplier)
                .Select(y => new { Supplier = y.Key, Tns = y.Sum(r => r.SalesSum) }).ToList();

            return supplierTns.ConvertAll(x => new JsonResult(x));
        }

         [HttpGet("GetAssortmentApriori")]
         public List<JsonResult> GetAssortmentApriori(string ids)
         {
             var listLong = ids.Split(',').Select(k=>Int64.Parse(k)).ToList();
             return Service.GetAprioriAssortment(listLong,  Context).ConvertAll(x=>new JsonResult(x));
         }

         [HttpGet("GetFrequentlyAssortment")]
         public List<JsonResult> GetFrequentlyAssortment()
         {
             return Service.GetFrequentlyAssortment( Context).ConvertAll(x => new JsonResult(x));
         }

    }
}
