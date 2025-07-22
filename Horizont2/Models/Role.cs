using System;
using System.Collections.Generic;

#nullable disable

namespace Horizont.Models
{
    public partial class Role:BaseModel
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
