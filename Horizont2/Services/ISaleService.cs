using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizont.Connection;
using Horizont.Models;

namespace Horizont.Services
{
    public interface ISaleService
    {
        List<Assortment> GetAprioriAssortment( List<long> ids);
    }
}
