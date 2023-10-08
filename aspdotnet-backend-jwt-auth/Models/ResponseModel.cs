namespace JWTBackendAuth.Models
{
    public struct GetResponse
    {
        public object Data { get; set; }
        public string Message { get; set; }
    }

    public struct PostResponse
    {
        public object Message { get; set; }
    }
}
