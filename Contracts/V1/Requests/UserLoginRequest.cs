using System;
namespace TodoApi.Contracts.V1.Requests
{
    public class UserLoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
