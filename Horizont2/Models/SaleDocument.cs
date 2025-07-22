using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

#nullable disable

namespace Horizont.Models
{
    public partial class SaleDocument:BaseModel
    {
        public SaleDocument()
        {
            Sales = new List<Sale>();
        }

        public string Division { get; set; }
        public string Warehouse { get; set; }
        public string WarehouseType { get; set; }
        public string DocumentName { get; set; }
        public string ManagerName { get; set; }
        public string Supplier { get; set; }
        public long? ContrpartnerId { get; set; }
        public DateTime? DocumentDate { get; set; }

        public virtual Contrpartner Contrpartner { get; set; }
        [JsonExtensionData]
        public virtual IList<Sale> Sales { get; set; }

        [JsonExtensionData]
        public virtual IList<Assortment> Assortments { get; set; }

        public List<Assortment> GetAssortments()
        {
            return Sales.Select(x => x.Assortment).ToList();
        }



    }
}
