using Microsoft.AspNetCore.Mvc;

namespace EKO.ConnectFour.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StatusController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return Json(new
        {
            Status = "OK",
            Message = "Hello from EKO.ConnectFour.Api!"
        });
    }
}
