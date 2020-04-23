using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTaskApi.Models
{
    public class User
    {
        private readonly HomeTasksContext _db;

        public int Id { get; set; }
        public String Name { get; set; }
        public String Password { get; set; }
        public UserRole Role { get; set; }
        public List<HomeTask> Tasks { get; set; }

        public User(HomeTasksContext context)
        {
            _db = context;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum UserRole
    {
        Master,
        Slave,
        None
    }
}
