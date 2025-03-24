using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Repositories;
using NewPlasmaDonorsAPI.Services;
using NewPlasmaDonorsAPI.Models;


namespace NewPlasmaDonorsAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    [EnableCors("AllowAll")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly ICompanyLocationRepository _companyService;

        public UserController(UserService userService, IUserRepository userRepository, IPasswordHasherService passwordHasherService, ICompanyLocationRepository companyService) // Add companyService parameter
        {
            _userService = userService;
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _companyService = companyService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticationRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid client request");
            }
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email cannot be null or empty");
            }

            return Ok("Authentication successful");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and Password are required" });
            }

            // Check if the email already exists
            var existingUser = await _userRepository.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email is already in use" });
            }

            // Hash the plain-text password using the PasswordHasherService
            var hashedPassword = _passwordHasherService.HashPassword(request.Password);

            // Create a new user object
            var newUser = new UserModel
            {
                email = request.Email,
                password = hashedPassword,
                username = request.Email // Assuming the email is used as the username
            };

            // Save the new user to the repository
            await _userRepository.AddUserAsync(newUser);

            // Return a success message
            return Ok(new { message = "User registered successfully" });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
        {
            //Console.WriteLine($"Login Request: Email={request.Email}, Password={request.Password}");

            var userDto = await _userService.ValidateUserAsync(request.Email, request.Password);
            if (userDto == null)
            {
                Console.WriteLine("Invalid credentials");
                return Unauthorized(new { message = "Invalid credentials" });
            }           

            return Ok(userDto);
        }



        [HttpPost("adduser")]
        public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
        {
            var response = await _userService.AddUserAsync(userDto);

            if (response == null)
            {
                return BadRequest(response);
            }
            
            return Ok(response);
        }

        [HttpGet("employees")]
        public async Task<ActionResult<List<UserDto>>> GetEmployees()
        {
            var employees = await _userService.FindAllEmployeesAsync();

            //Test
            return Ok(employees);
        }

        [HttpGet("getemployees")]
        public ActionResult<List<UserModel>> GetEmployeeList()
        {
            var users = _userService.GetEmployeeList();
            if (users == null || users.Count == 0)
            {
                return NotFound(new { message = "No employees found" });
            }
            return Ok(users);
        }

        [HttpGet("company/locations")]
        public ActionResult<ResInfo> GetCompanyLocations()
        {
            var response = _userService.CompanyLocations();
            return Ok(response);
        }

        [HttpGet("role")]
        public IActionResult GetRole()
        {
            var response = _userService.GetRoles();
            return Ok(response);
        }

        [HttpPut("update/id/{id}")]
        public IActionResult UpdateUser([FromRoute] long id, [FromBody] UserModel userModel)
        {
            if (id <= 0 || userModel == null)
            {
                return BadRequest("Invalid input.");
            }

            var result = _userService.UpdateUser(userModel, id);
            return Ok(result);
        }

      

        [HttpGet("delete-user/{id}")]
        public IActionResult DeleteUser([FromRoute] long id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            _userService.DeleteUser(id);
            return Ok("User successfully marked as deleted.");
        }

        [HttpGet("fetchUserDetailsById/id/{id}")]
        public IActionResult FetchUserDetailsById([FromRoute] long id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var user = _userService.FetchUserDetailsById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user);
        }


    }
}

