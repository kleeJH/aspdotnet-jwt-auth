namespace JWTBackendAuth.Models
{
    public struct LoginModel
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }

    public struct UserModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserCreatedModel
    {
        public bool Succeeded { get; set; }
        public List<string> Message = new();
    }
}

