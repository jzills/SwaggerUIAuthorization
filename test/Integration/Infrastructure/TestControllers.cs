using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SwaggerUIAuthorization.Integration.Tests.Infrastructure;

[ApiController]
[Route("api/open")]
public class OpenController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();
}

[ApiController]
[Route("api/anonymous")]
public class AnonymousController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get() => Ok();
}

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Get() => Ok();
}

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "User")]
    public IActionResult Get() => Ok();
}

[ApiController]
[Route("api/policy")]
public class PolicyController : ControllerBase
{
    [HttpGet]
    [Authorize("TestPolicy")]
    public IActionResult Get() => Ok();
}

[ApiController]
[Route("api/multirole")]
public class MultiRoleController : ControllerBase
{
    // Comma-separated roles are evaluated with OR semantics
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public IActionResult Get() => Ok();
}
