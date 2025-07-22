using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private ISaleService SaleService { get; set; }
        private IBaseRepository<Contrpartner> Contrpartners { get; set; }
        private IBaseRepository<Assortment> Assortments { get; set; }

        public HomeController(ILogger<HomeController> logger, ApplicationContext dbContext)
        {
            Context = dbContext;
            _logger = logger;
        }

        [HttpGet("GetContrpartnerFirst")]
        public JsonResult GetContrpartnerFirst()
        {
            return new JsonResult(Context.GetContrpartners().FirstOrDefault());
        }

        [HttpGet("GetContrpartnerByName")]
        public List<JsonResult> GetContrpartnerByName(String name)
        {
            return Context.Contrpartners
                .Where(x => x.ContrpartnerName.ToLower().Contains(name.ToLower()))
                .ToList().ConvertAll(x => new JsonResult(x));
        }

        [HttpGet("GetAssortmentsByContrpartner")]
        public List<JsonResult> GetAssortmentsByContrpartner(long id)
        {
            var list = Context.SaleDocuments
                .Where(x => x.ContrpartnerId == id)
                .ToList();
            list.ForEach(x => x.Assortments = Context.GetAssortmentBySaleDocument(x.Id));
            return list.ConvertAll(x => new JsonResult(x));
        }

        [HttpGet("GetSalesBySaleDocument")]
        public List<JsonResult> GetSalesBySaleDocument(long id)
        {
            var list = Context.Sales
                .Where(x => x.SaleDocumentId == id)
                .ToList();

            list.ForEach(x => x.Assortment = Context.Assortments.FirstOrDefault(z => z.Id == x.AssortmentId));

            return list.ConvertAll(x => new JsonResult(x));
        }

        [HttpGet]
        public List<JsonResult> GetAssortment()
        {
            return Context.Assortments
                .Take(200)
                .ToList().ConvertAll(x => new JsonResult(x));
        }

        [HttpGet("GetAssortmentApriori")]
        public List<JsonResult> GetAssortmentApriori(List<long> ids)
        {
            return SaleService.GetAprioriAssortment(ids).ConvertAll(x => new JsonResult(x));
        }

    }
}
