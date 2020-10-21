using System;
using System.Collections.Generic;

namespace TodoApi.Domains
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public bool success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
