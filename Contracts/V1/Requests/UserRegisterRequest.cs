﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Contracts.V1.Requests
{
    public class UserRegisterRequest
    {
        [EmailAddress]
        public string email { get; set; }
        public string password { get; set; }
    }
}
