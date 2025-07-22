using System;
using System.Collections.Generic;

#nullable disable

namespace Horizont.Models
{
    public partial class User:BaseModel
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public long? RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
