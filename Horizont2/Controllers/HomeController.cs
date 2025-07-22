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
        private ISaleService SaleService { get; set; }
        private IBaseRepository<Contrpartner> Contrpartners { get; set; }
        private IBaseRepository<Assortment> Assortments { get; set; }
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public JsonResult GetContrpartners()
        {
            return new JsonResult(Contrpartners.GetAll());
        }

        [HttpGet]
        public JsonResult GetAssortments()
        {
            return new JsonResult(Assortments.GetAll());
        }

        [HttpGet]
        public JsonResult GetAssortmentApriori(List<long> ids)
        {
            return new JsonResult(null); //return new JsonResult(SaleService.GetAprioriAssortment(ids));
        }

    }
}
