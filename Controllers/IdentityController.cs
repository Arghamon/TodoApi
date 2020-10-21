using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Contracts.V1;
using TodoApi.Contracts.V1.Requests;
using TodoApi.Services; 

namespace TodoApi.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            return Ok();
        }
    }
}
