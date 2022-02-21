using Microsoft.AspNetCore.Mvc;
using newsApi.Data;
using newsApi.Models;
using System.Threading.Tasks;

namespace newsApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/CreateUserAsync
        [HttpPost]
        public Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.ExpoNotificationRequest))
            {
                _userService.CreateUserAsync(user);
            }
            return Task.FromResult<IActionResult>(Ok());
        }
    }
}