using Newtonsoft.Json;
using JWTBackendAuth.Models;

namespace JWTBackendAuth.Utilities
{
    public static class ResponsePackager
    {
        public static string GetResponse(string message, object data)
        {
            GetResponse response = new()
            {
                Data = data,
                Message = message
            };


            return Serialize(response);
        }

        public static string PostResponse(object message)
        {
            PostResponse response = new()
            {
                Message = message
            };


            return Serialize(response);
        }

        private static string Serialize(object response)
        {
            return JsonConvert.SerializeObject(response);
        }
    }
}
