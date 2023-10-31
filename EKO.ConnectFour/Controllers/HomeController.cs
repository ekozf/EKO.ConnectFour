using EKO.ConnectFour.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EKO.ConnectFour.Api.Controllers;

//DO NOT TOUCH THIS FILE!!
[Route("")]
[AllowAnonymous]
public class HomeController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
#if DEBUG
        return Redirect("~/swagger");
#else
        return Redirect("/ping");
#endif
    }

    [HttpGet("ping")]
    [ProducesResponseType(typeof(PingResultModel), (int)HttpStatusCode.OK)]
    public IActionResult Ping()
    {
        var model = new PingResultModel
        {
            IsAlive = true,
            Greeting = $"Hello on this fine {DateTime.Now.DayOfWeek}",
            ServerTime = DateTime.Now
        };

        return Ok(model);
    }
}