using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Sample.Authentication;

namespace SwaggerUIAuthorization.Controllers;

[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AuthenticationController : Controller
{
    [HttpGet]
    [Route("login")]
    public IActionResult Login() => View();

    // This is for demonstration purposes only. 
    // Of course, you should NEVER allow a user to arbitrarily pass a role 
    // for themselves as a query string parameter nor should you pass a 
    // role as a query string parameter to begin with. Or even have a mutatable role exposed
    // client side like this.
    [HttpGet]
    [Route("authenticate")]
    public async Task<IActionResult> Authenticate(string role)
    {
        var claims = new[] 
        { 
            new Claim(ClaimTypes.Role, role),
            // new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, AuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(AuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        return Ok();
    }
}