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
        public TaskType Type {get; set;}
        public int? UserId { get; set; }
        public User User { get; set; }
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
public enum TaskType
{
    MasterTask,
    SlaveTask
}
