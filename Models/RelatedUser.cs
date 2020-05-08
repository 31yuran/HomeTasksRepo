using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTaskApi.Models
{
    public class RelatedUser
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String SharedPassword { get; set; }
        public int OwnUserId { get; set; }
    }
}
