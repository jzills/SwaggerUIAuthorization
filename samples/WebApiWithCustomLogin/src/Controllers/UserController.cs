using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sample.Models;

namespace Sample.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User")]
public class UserController : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public IActionResult Get(Guid id) => Ok();

    [HttpPost]
    public IActionResult Post([FromBody] User request) => Ok();

    [HttpPut]
    public IActionResult Put([FromBody] User request) => Ok();

    [HttpDelete]
    [Route("{id}")]
    [Authorize("CanDeletePolicy")]
    public IActionResult Delete(Guid id) => Ok();
}
