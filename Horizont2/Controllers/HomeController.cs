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
            Service = Service;
        }

        [HttpGet("GetContrpartnerByName")]
        public List<JsonResult> GetContrpartnerByName(String name)
        {
            return Context.Contrpartners.Where(x => x.ContrpartnerName.ToLower().Contains(name.ToLower())).ToList()
                .ConvertAll(x => new JsonResult(x));
        }
        [HttpGet("GetAssortments")]
         public List<JsonResult> GetAssortments()
         {
             return Context.Assortments.Take(500).ToList().ConvertAll(x => new JsonResult(x));
         }

         [HttpGet("GetTnsByContrpartner")]
         public JsonResult GetTnsByContrpartner(long id)
         {
             var documentIds = Context.SaleDocuments.Where(x => x.ContrpartnerId == id).Select(y=>y.Id).ToList();
             return new JsonResult(Context.Sales.Where(x => documentIds.Contains(x.SaleDocumentId.GetValueOrDefault()))
                 .Select(y => y.Tns).Sum());
         }

        [HttpGet("GetAssortmentApriori")]
         public List<JsonResult> GetAssortmentApriori(List<JsonResult> assortments)
         {
             var ids = new List<long> {5, 9};//assortments.Select(x=>x.SerializerSettings(YieldAwaitable=>y.))
             return Service.GetAprioriAssortment(ids).ConvertAll(x=>new JsonResult(x));
         }

    }
}
