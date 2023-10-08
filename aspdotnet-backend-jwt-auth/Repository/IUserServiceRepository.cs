using Microsoft.AspNetCore.Mvc;
using JWTBackendAuth.Models;
using Microsoft.AspNetCore.Identity;

namespace JWTBackendAuth.Repository
{
    public interface IUserServiceRepository
    {
        Task<UserCreatedModel> CreateUserAsync(UserModel newUser);

        Task<bool> IsEmailOrUsername(string id);

        Task<bool> IsValidUserAsync(LoginModel users);

        Task<IdentityUser> GetIdentityUser(string id);

        RefreshTokenModel AddUserRefreshTokens(RefreshTokenModel user);

        RefreshTokenModel? GetSavedRefreshTokens(string username, string refreshtoken);

        void DeleteUserRefreshTokens(string username, string refreshToken);

        int SaveCommit();
    }
}
