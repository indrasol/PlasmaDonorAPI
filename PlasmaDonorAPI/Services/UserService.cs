using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Services;
using NewPlasmaDonorsAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DocumentFormat.OpenXml.Spreadsheet;
using NewPlasmaDonorsAPI.utils;
using EnvDTE;
using Microsoft.VisualStudio.OLE.Interop;

namespace NewPlasmaDonorsAPI.Services
{
    public class UserService : BaseService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ICompanyLocationRepository _companyLocationRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtService _jwtService;

        public UserService(ILogger<UserService> logger, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IPasswordHasherService passwordHasher, ICompanyLocationRepository companyLocationRepository, IRoleRepository roleRepository, IJwtService jwtService)
        : base(logger, httpContextAccessor)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _companyLocationRepository = companyLocationRepository;
            _roleRepository = roleRepository;
            _jwtService = jwtService;
        }



        public async Task<ResInfo?> ValidateUserAsync(string email, string password)
        {
            // Find the user by email
            var user = await _userRepository.FindByEmailAsync(email);

            // Check if the user exists and has a valid password hash
            if (user == null || string.IsNullOrEmpty(user.password))
            {
                return null; // User not found or password hash is missing
            }

            // Verify the password using the password hasher
            var isPasswordValid = _passwordHasher.VerifyPassword(user.password, password);

            if (isPasswordValid)
            {
                UserDto userDto = new UserDto();
                userDto.token = _jwtService.GenerateToken(email);
                userDto.role = _roleRepository.GetById(user.roleId)?.role;
                return Success(userDto);
            }


            return null;
        }



        public async Task RegisterUserAsync(string email, string password)
        {
            var hashedPassword = _passwordHasher.HashPassword(password);
            var user = new UserModel { email = email, password = hashedPassword };
            await _userRepository.AddUserAsync(user);
        }

        public List<UserModel> GetEmployeeList()
        {
            return _userRepository.FindAllUsersByDeletedFalse();
        }

        public ResInfo CompanyLocations()
        {
            // Fetch all locations from the repository
            List<CompanyLocation> locations = (List<CompanyLocation>)_companyLocationRepository.GetAllLocations(_companyLocationRepository.Get_context());

            // Return the response wrapped in a ResInfo object
            return Success(locations);
        }

        // Assuming Success is a method in your service that wraps the result in ResInfo

        public ResInfo Success(object data)
        {
            return new ResInfo
            {
                //Success = true,
                Data = data,
                Status = true 
            };
        }

        public ResInfo GetRoles()
        {
            // Fetch roles from the repository
            var roles = _roleRepository.GetAllRoles();

            // Wrap the roles in a ResInfo object
            return new ResInfo
            {
                Status = true,
                Data = roles,
                Msg = "Roles fetched successfully"
            };
        }


        public async Task<ResInfo> AddUserAsync(UserDto info)
        {
            if (!NullUtils.IsValid(info.email))
            {
                return Error( "Invalid data: email is invalid");
            }

            UserModel? updateUser = null;

            if (NullUtils.IsValid(info.id))
            {
                updateUser = _userRepository.GetById(info.id);
            }
            UserModel? duplicateUser = await _userRepository.FindByEmailAsync(info.email);
            if (duplicateUser != null) {
                if (updateUser != null && duplicateUser.id != updateUser.id && !duplicateUser.email.Equals(updateUser.email, StringComparison.OrdinalIgnoreCase)) {
                    return Error("Email cannot be changed");
                }
                else if( updateUser == null)
                {
                    return Error("Email already exists");
                }
            }

            UserModel userModel = UserMapper.MapToUserModel(info);

            string? passwordtoHash = null;
            if (NullUtils.IsValid(info.password))
            {
                passwordtoHash = info.password;
            }
            else if (updateUser != null)
            {
                passwordtoHash = updateUser.password;
            }

            if(string.IsNullOrEmpty(passwordtoHash))
            {
                return Error("Password is required");
            }

            userModel.password = BCrypt.Net.BCrypt.HashPassword(passwordtoHash);                   

            if (updateUser != null)
            {
                userModel.id = updateUser.id;
                userModel.createdOn = updateUser.createdOn;
                userModel.createdBy = updateUser.createdBy;

                userModel.updatedOn = DateTime.Now;
                userModel.updatedBy = GetLoggedUserId();
            }
            else
            {
                userModel.createdOn = DateTime.UtcNow;
                userModel.createdBy = GetLoggedUserId();
            }           

            if (NullUtils.IsValid(info.companyLocationId))
            {
                CompanyLocation? cl = _companyLocationRepository.GetById(info.companyLocationId);
                if (cl != null)
                {
                    userModel.companyLocation = cl.id;
                }
            }

            if (NullUtils.IsValid(info.roleId))
            {
                Roles? role = _roleRepository.GetById(info.roleId);
                if (role != null)
                {
                    userModel.roleId = role.id;
                    userModel.role = role.title;
                    userModel.roleType = role.role;
                }
            }
            
            UserModel? savedUser = null;
            try
            {               
                savedUser = _userRepository.Save(userModel);
            }
            catch (Exception e)
            {
                return new ResInfo { Data = "Error while saving into Database: " + e.Message };
            }

            return Success(savedUser.id);
        }

        public ResInfo DeleteUser(long id)
        {
            var user = _userRepository.GetById(id);

            if (user == null)
            {
                return new ResInfo { Data = $"User with ID {id} not found.", Status = false };
            }

            user.deleted = true;
            _userRepository.Save(user);

            return new ResInfo { Data = "Deleted", Status = true };
        }

        public string UpdateUser(UserModel updatedModel, long id)
        {
            // Fetch the user from the database
            var updateUser = _userRepository.GetById(id);

            // Update the user's fields
            updateUser.firstName = updatedModel.firstName;
            updateUser.lastName = updatedModel.lastName;
            updateUser.email = updatedModel.email;
            updateUser.phoneNumber = updatedModel.phoneNumber;
            updateUser.role = updatedModel.role;

            // Save the updated user to the database
            _userRepository.Save(updateUser);

            return "User details updated successfully.";
        }

        public UserModel FetchUserDetailsById(long id)
        {
            return _userRepository.GetById(id);
        }

        public async Task<List<UserDto>> FindAllEmployeesAsync()
        {
            var employees = await _userRepository.FindAllByRoleTypeAndDeletedAsync();



            // Convert List<UserModel> to List<UserDto>
            var employeeDtos = employees.Select(e => new UserDto
            {
                id = e.id,         // Ensure lowercase properties
                firstName = e.firstName,
                email = e.email,
                password = "Unchanged$2",   // Nullify password for security
                lastName = e.lastName,
                username = e.username,
                phoneNumber = e.phoneNumber,
                roleType = e.roleType,
                status = e.status,
                createdOn = DateUtils.ToShortString(e.createdOn),
                updatedOn = DateUtils.ToShortString(e.updatedOn),
                companyId = e.company,
                roleId = e.roleId,
                companyLocationId = e.companyLocation

            }).ToList();

            return employeeDtos;
        }


        // Get the count of active users
        public long GetActiveUserCount()
        {
            return _userRepository.GetActiveUserCount();
        }

        // Get the count of active employees
        public long GetActiveEmployeesCount()
        {
            return _userRepository.GetActiveEmployeesCount();
        }

        // Get the count of admins
        public long GetAdminCount()
        {
            return _userRepository.GetAdminCount();
        }

        //public ResInfo GetCompanyLocations()
        //{
        //    var locs = _ccompanyLocationRepository.GetAll(); // Assuming GetAll() fetches all locations

        //    var clInfos = locs.Select(l => new CompanyLocationInfo
        //    {
        //        Id = l.Id,
        //        FullAddress = l.FullAddress
        //    }).ToList();

        //    return Success(clInfos); // Assuming Success() is a helper method similar to Java
        //}

        //public ResInfo Roles()
        //{
        //    var roles = _roleRepository.GetAll(); // Assuming GetAll() fetches all roles
        //    return Success(roles);
        //}

    }
    }
