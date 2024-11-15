using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sample.Models;

namespace Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class AdministratorController : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles = "Administrator")]
    public IActionResult Get(Guid id) => Ok();

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public IActionResult Post([FromBody] Administrator request) => Ok();

    [HttpPut]
    [Authorize(Roles = "Administrator")]
    public IActionResult Put([FromBody] Administrator request) => Ok();

    [HttpDelete]
    [Route("{id}")]
    [Authorize("CanDeletePolicy")]
    public IActionResult Delete(Guid id) => Ok();
}