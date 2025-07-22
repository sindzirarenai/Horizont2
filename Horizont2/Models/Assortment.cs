using System;
using System.Collections.Generic;

#nullable disable

namespace Horizont.Models
{
    public partial class Assortment: BaseModel
    {
        public Assortment()
        {
            //Sales = new List<Sale>();
        }

        public string AssortmentCategory { get; set; }
        public string AssortmentName { get; set; }
        public string Standard { get; set; }
        public string SteelGrade { get; set; }
        public int? Diameter { get; set; }
        public int? Diameter2 { get; set; }
        public int? Wall { get; set; }
        public int? Wall2 { get; set; }
        //public IEnumerable<Sale> Sales { get; set; }
    }
}
