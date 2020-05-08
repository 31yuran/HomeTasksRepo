using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTaskApi.Models;
using HomeTaskApi.Services;
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
        private readonly UserService _userService;
        private readonly TaskService _taskService;
        private readonly IHubContext<MyHub> _hubContext;

        public HomeTasksController(UserService userService, TaskService taskService, IHubContext<MyHub> hubContext)
        {
            _userService = userService;
            _taskService = taskService;
            _hubContext = hubContext;

            /*if (!_userService.IsHaveAnyUsers)
            {
                var m = _userService.AddUser(new User() { Name = "master", Password = "1111", SlavePassword = "slave" });
                var s = _userService.AddUser(new User() { Name = "Koza", Password = "2222", SlavePassword = "slave" });

                _taskService.AddTask(new HomeTask() { User = m, Type = TaskType.MasterTask, Desc = "MasterTask", EndOfExecution = DateTime.Now.AddMinutes(5) });
                _taskService.AddTask(new HomeTask() { User = s, Type = TaskType.SlaveTask, Desc = "SlaveTask", EndOfExecution = DateTime.Now.AddMinutes(5) });
            }*/
        }

        #region USER API
        // GET: api/<controller>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await _userService.GetUsers();
        }

        // GET api/<controller>/5
        [HttpGet]
        [Route("user/{id}")]
        public async Task<ActionResult<User>> Get([FromRoute] int id)
        {
            var user = await _userService.GetUser(id);
            if (user == null) return StatusCode(204, $"User with id '{id}' not found.");
            user.Slaves = _userService.GetUserSlaves(user);
            return new ObjectResult(user);
        }

        //[HttpGet("{userName}/{password}")]
        [HttpGet]
        [Route("user/{userName}/{password}")]
        public async Task<ActionResult<User>> Get([FromRoute] string userName, string password)
        {
            if (!_userService.ExistByName(userName))
                return StatusCode(204, $"User with name '{userName}' not found.");
            var user = await _userService.GetUser(userName, password);
            if (user == null) return StatusCode(204, $"wrong password");
            user.Slaves = _userService.GetUserSlaves(user);
            return new ObjectResult(user);
        }

        [HttpPost]
        [Route("user")]
        public async Task<ActionResult<User>> Post(User user)
        {
            if (String.IsNullOrWhiteSpace(user.Name)
                || String.IsNullOrWhiteSpace(user.Password)
                || String.IsNullOrWhiteSpace(user.SlavePassword))
                return BadRequest();

            if (_userService.ExistByName(user.Name))
                return StatusCode(409, $"User '{user.Name}' already exists.");

            var newUser = await _userService.AddUser(user);
            //await _hubContext.Clients.All.SendAsync("addUserHandle");
            return Ok(newUser);
        }

        [HttpGet]
        [Route("slaves")]
        public async Task<ActionResult<IEnumerable<RelatedUser>>> GetSlaves(int masterId)
        {
            if (!_userService.ExistById(masterId))
                return StatusCode(204, $"User not found.");
            var user = await _userService.GetUser(masterId);
            if (user == null) return StatusCode(204, $"wrong password");
            return _userService.GetUserSlaves(user);
        }

        [HttpPut]
        [Route("slaves")]
        public async Task<ActionResult<User>> Put(RelatedUser slave)
        {
            if (slave == null) return BadRequest();

            var newSlave = await _userService.GetUserBySharedPassword(slave.Name, slave.SharedPassword);
            if (newSlave == null) return StatusCode(204, "slave not found.");

            var ownUser = await _userService.GetUser(slave.OwnUserId);
            if(ownUser == null) return StatusCode(204, "user not found.");

            ownUser = await _userService.AddSlave(ownUser, newSlave.Id);
            //await _hubContext.Clients.All.SendAsync("changeTaskHandle");
            return Ok(ownUser);
        }
        #endregion

        #region Task API
        [HttpGet]
        [Route("tasks/{userId}")]
        public async Task<ActionResult<IEnumerable<HomeTask>>> GetTasks([FromRoute] int userId)
        {
            var user = await _userService.GetUser(userId);
            if(user == null) return StatusCode(204, "user not found.");

            var slaveTasks = await _taskService.GetTasks(user);
            var allTasks = new List<HomeTask>();
            allTasks.AddRange(slaveTasks.Value);

            var slaves = _userService.GetUserSlaves(user);
            foreach (var slave in slaves)
            {
                var userSlave = await _userService.GetUser(slave.Id);
                if (userSlave == null) continue;
                var masterTasks = await _taskService.GetTasks(userSlave);
                allTasks.AddRange(masterTasks.Value);
            }
            return Ok(allTasks);
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<ActionResult<HomeTask>> Post(HomeTask homeTask)
        {
            if (homeTask == null) return BadRequest();
            var newTask = await _taskService.AddTask(homeTask);
            await _hubContext.Clients.All.SendAsync("addTaskHandle");
            return Ok(newTask);
        }

        [HttpPut]
        public async Task<ActionResult<HomeTask>> Put(HomeTask homeTask)
        {
            if (homeTask == null) return BadRequest();
            if (!_taskService.ExistById(homeTask.Id)) return NotFound();

            var t = await _taskService.UpdateTask(homeTask);
            await _hubContext.Clients.All.SendAsync("changeTaskHandle");
            return Ok(t);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<HomeTask>> Delete(int id)
        {
            var homeTask = _taskService.GetById( id);
            if (homeTask == null) return NotFound();

            _taskService.DelTask(homeTask);
            await _hubContext.Clients.All.SendAsync("removeTaskHandle");
            return Ok(homeTask);
        }
        #endregion
    }

    public class MyHub : Hub{}
}
