using System;
using System.Collections.Generic;

#nullable disable

namespace Horizont.Models
{
    public partial class Contrpartner:BaseModel
    {
        public Contrpartner()
        {
            SaleDocuments = new HashSet<SaleDocument>();
        }

        public string ContrpartnerName { get; set; }
        public long? ContrpartnerInn { get; set; }
        public string ContrpartnerType { get; set; }
        public string ContrpartnerStatus { get; set; }
        public string LevelSale { get; set; }

        public virtual ICollection<SaleDocument> SaleDocuments { get; set; }
    }
}
