using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExampleWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            // Retrieve the previously created JWT.
            var jwt = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrEmpty(jwt))
            {
                return Unauthorized();
            }

            return Ok(jwt);
        }
    }
}