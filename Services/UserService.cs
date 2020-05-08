using HomeTaskApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTaskApi.Services
{
    public class UserService
    {
        private readonly HomeTasksContext _context;

        public bool IsHaveAnyUsers { get => _context.Users.Any(); }

        public UserService(HomeTasksContext context)
        {
            _context = context;
        }

        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(x => x.Tasks).ToListAsync();
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }

        public async Task<User> GetUser(string userName, string password)
        {
            var user = await _context.Users.Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Name == userName && x.Password == password);
            return user;
        }
        public async Task<User> GetUserBySharedPassword(string userName, string sharedPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Name == userName && x.SlavePassword == sharedPassword);
            return user;
        }

        public bool ExistByName(String name) { return _context.Users.Any(x => x.Name == name); }
        public bool ExistById(int id) { return _context.Users.Any(x => x.Id == id); }

        public async Task<User> AddSlave(User user, int slaveId)
        {
            if (!String.IsNullOrWhiteSpace(user.SlavesId))
            {
                var ownUserslavesId = user.SlavesId.Split(';');
                if (ownUserslavesId.Any(x => x == user.Id.ToString())) return user;
            }
            user.SlavesId += $"{slaveId};";
            _context.Update(user);
            await _context.SaveChangesAsync();
            user.Slaves = GetUserSlaves(user);
            return user;
        }
        public List<RelatedUser> GetUserSlaves(User user)
        {
            var slaves = new List<RelatedUser>();
            if (String.IsNullOrWhiteSpace(user.SlavesId)) return slaves;

            var slavesIdParts = user.SlavesId.Split(';').Where(x => !String.IsNullOrWhiteSpace(x));
            foreach (var sp in slavesIdParts)
            {
                if (Int32.TryParse(sp, out int idInt))
                {
                    var slave = _context.Users.FirstOrDefault(x => x.Id == idInt);
                    if (slave != null) slaves.Add(new RelatedUser() { Id = idInt, Name = slave.Name, SharedPassword = slave.SlavePassword });
                }
            }
            return slaves;
        }
    }
}
