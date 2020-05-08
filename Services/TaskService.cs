using HomeTaskApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTaskApi.Services
{
    public class TaskService
    {
        private readonly HomeTasksContext _context;
        public TaskService(HomeTasksContext context)
        {
            _context = context;
        }
        public HomeTask GetById(int id) { return _context.HomeTasks.FirstOrDefault(x => x.Id == id); }

        public async Task<HomeTask> AddTask(HomeTask task)
        {
            _context.HomeTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public Boolean ExistById(int id) { return _context.HomeTasks.Any(x => x.Id == id); }

        public async Task<ActionResult<IEnumerable<HomeTask>>> GetTasks(User user)
        {
            return await _context.HomeTasks.Where(x=>x.UserId == user.Id).ToListAsync();
        }
        public async Task<HomeTask> UpdateTask(HomeTask task)
        {
            _context.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async void DelTask (HomeTask task)
        {
            _context.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
