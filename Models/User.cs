using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTaskApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Password { get; set; }
        public String SlavePassword { get; set; }
        public List<HomeTask> Tasks { get; set; }
        public String SlavesId { get; set; }
        [NotMapped]
        public List<RelatedUser> Slaves { get; set; }
    }
}
