using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using JWTBackendAuth.Models;

namespace JWTBackendAuth.Repository
{
    public class UserServiceRepository : IUserServiceRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _db;
        private readonly IValidator<string> _validator;

        public UserServiceRepository(UserManager<IdentityUser> userManager, AppDbContext db, IValidator<string> validator)
        {
            this._userManager = userManager;
            this._db = db;
            this._validator = validator;
        }

        /// <summary>
        /// Uses the AppDbContext to add RefreshTokenModel into the DB
        /// </summary>
        /// <param name="newRefreshToken">RefreshTokenModel object</param>
        /// <returns></returns>
        public RefreshTokenModel AddUserRefreshTokens(RefreshTokenModel newRefreshToken)
        {
            _db.UserRefreshToken.Add(newRefreshToken);
            return newRefreshToken;
        }

        /// <summary>
        /// Deletes a specific refresh token from the DB using AppDbContext
        /// </summary>
        /// <param name="username"></param>
        /// <param name="refreshToken"></param>
        public void DeleteUserRefreshTokens(string username, string refreshToken)
        {
            var item = _db.UserRefreshToken.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken);
            if (item != null)
            {
                _db.UserRefreshToken.Remove(item);
            }
        }

        /// <summary>
        /// Gets an active specific user refresh token from the DB using AppDbContext
        /// </summary>
        /// <param name="username"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public RefreshTokenModel? GetSavedRefreshTokens(string username, string refreshToken)
        {
            return _db.UserRefreshToken.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken && x.IsActive == true);
        }

        /// <summary>
        /// Commits changes made to the DB using AppDbContext
        /// </summary>
        /// <returns></returns>
        public int SaveCommit()
        {
            return _db.SaveChanges();
        }

        /// <summary>
        /// Checks if the parameter id is an email or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsEmailOrUsername(string id)
        {
            ValidationResult isEmail = await _validator.ValidateAsync(id);

            return isEmail.IsValid;
        }

        /// <summary>
        /// Uses the UserManager to check if it is a valid user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<bool> IsValidUserAsync(LoginModel login)
        {
            IdentityUser? user = await GetIdentityUser(login.Id);

            // Might need to check for problems
            var result = await _userManager.CheckPasswordAsync(user!, login.Password);
            return result;
        }

        /// <summary>
        /// Get IdentityUser using the user's username or email
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IdentityUser> GetIdentityUser(string id)
        {
            IdentityUser? user;

            // If using Email, search user using Email. Else, search using username
            if (await IsEmailOrUsername(id))
            {
                user = await _userManager.FindByEmailAsync(id);
            }
            else
            {
                user = await _userManager.FindByNameAsync(id);
            }

            return user!;
        }

        /// <summary>
        /// Uses the UserManager to create a new user in the DB
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public async Task<UserCreatedModel> CreateUserAsync(UserModel newUser)
        {
            UserCreatedModel output = new UserCreatedModel();

            // Create Identity
            IdentityUser userIdentity = new()
            {
                UserName = newUser.UserName,
                NormalizedUserName = newUser.UserName.ToUpper(),
                Email = newUser.Email,
                NormalizedEmail = newUser.Email.ToUpper()
            };

            // Update in DB
            var result = await _userManager.CreateAsync(userIdentity, newUser.Password);

            // Return results
            output.Succeeded = result.Succeeded;

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    output.Message.Add(error.Description);
                }
            }

            return output;
        }
    }
}
