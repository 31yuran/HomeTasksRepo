using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTaskApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using HomeTask = HomeTaskApi.Models.HomeTask;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace HomeTaskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeTasksController : Controller
    {
        private readonly HomeTasksContext _db;
        private readonly IHubContext<MyHub> _hubContext;

        public HomeTasksController(HomeTasksContext context, IHubContext<MyHub> hubContext)
        {
            _db = context;
            _hubContext = hubContext;
            if (!_db.HomeTasks.Any())
            {
                var m = new Master() { Name = "master" };
                _db.Masters.Add(m);
                _db.SaveChanges();
                var s = new Slave() { Name = "Koza" };
                _db.Slaves.Add(s);
                _db.SaveChanges();

                _db.HomeTasks.Add(new HomeTask { Master = m, Guid = Guid.NewGuid(), Slave = s, Desc = "Wake up" });
                _db.SaveChanges();
            }
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HomeTask>>> Get()
        {
            return await _db.HomeTasks.Include(x => x.Master).Include(x => x.Slave).ToListAsync();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HomeTask>> Get(int id)
        {
            var user = await _db.HomeTasks.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();
            return new ObjectResult(user);
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<ActionResult<HomeTask>> Post(HomeTask homeTask)
        {
            if (homeTask == null) return BadRequest();
            _db.HomeTasks.Add(homeTask);
            await _db.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("addTaskHandle");
            return Ok(homeTask);
        }
        [HttpPut]
        public async Task<ActionResult<HomeTask>> Put(HomeTask homeTask)
        {
            if (homeTask == null) return BadRequest();
            if (!_db.HomeTasks.Any(x => x.Id == homeTask.Id)) return NotFound();

            _db.Update(homeTask);
            await _db.SaveChangesAsync();
            return Ok(homeTask);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<HomeTask>> Delete(int id)
        {
            var homeTask = _db.HomeTasks.FirstOrDefault(x => x.Id == id);
            if (homeTask == null) return NotFound();

            _db.HomeTasks.Remove(homeTask);
            await _db.SaveChangesAsync();
            return Ok(homeTask);
        }
    }

    public class MyHub : Hub
    {
    }
}
