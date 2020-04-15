using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTaskApi.Models
{
    public class Slave
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public List<HomeTask> Tasks { get; set; }
        
    }
}
