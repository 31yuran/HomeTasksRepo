using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTaskApi.Models
{
    public class HomeTask
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int? MasterId { get; set; }
        public Master Master { get; set; }
        public int? SlaveId { get; set; }
        public Slave Slave { get; set; }
        public String Desc { get; set; }
        public DateTime TimeToComplete { get; set; }
        public DateTime StartOfExecution { get; set; }
        public DateTime EndOfExecution { get; set; }
        public TaskState State { get; set; }
        public Double Cost { get; set; }
    }
}

public enum TaskState
{
    Created = 0,
    Assigned,
    Complete,
    NonComplete,
    Verified, 
    NonVerified
}
