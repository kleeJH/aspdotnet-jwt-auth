using JWTBackendAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace JWTBackendAuth.Repository
{
    public interface IJWTManagerRepository
    {
        TokenModel? GenerateToken(string username);
        TokenModel? GenerateRefreshToken(string username);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
