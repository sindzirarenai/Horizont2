using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#nullable disable

namespace Horizont.Models
{
    public partial class Sale:BaseModel
    {
        public decimal? Tns { get; set; }
        public long? SaleDocumentId { get; set; }
        public long? AssortmentId { get; set; }

        [JsonExtensionData]
        public virtual Assortment Assortment { get; set; }
        public virtual SaleDocument SaleDocument { get; set; }
    }
}
