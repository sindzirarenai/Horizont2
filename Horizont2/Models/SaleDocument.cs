using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace Horizont.Models
{
    public partial class SaleDocument:BaseModel
    {
        public SaleDocument()
        {
            Sales = new HashSet<Sale>();
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
        public virtual ICollection<Sale> Sales { get; set; }

        public List<Assortment> GetAssortments()
        {
            return Sales.Select(x => x.Assortment).ToList();
        }

    }
}
