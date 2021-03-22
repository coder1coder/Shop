using Microsoft.AspNetCore.Mvc;
using Shop.WebApi.ResourceModels;
using Shop.WebApi.Services;

namespace Shop.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<bool> Login(LoginRequest request)
        {
            return Ok(_authService.Login(request.LastName));
        }
    }
}
