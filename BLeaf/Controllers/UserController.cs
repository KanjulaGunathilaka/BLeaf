using Microsoft.AspNetCore.Mvc;
using BLeaf.Models;
using BLeaf.Models.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLeaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = _userRepository.AllUsers.ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userRepository.FindUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set a default password hash if not provided
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = "default_password_hash"; // Replace with actual password hashing logic
            }

            var createdUser = await _userRepository.SaveUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null || user.UserId != id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedUser = await _userRepository.UpdateUser(user);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var deletedUser = await _userRepository.DeleteUser(id);
                return Ok(deletedUser);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}