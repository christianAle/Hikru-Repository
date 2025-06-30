using Microsoft.AspNetCore.Mvc;

namespace HikruCodeChallenge.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    /// <summary>
    /// Welcome endpoint that redirects to Swagger UI
    /// </summary>
    /// <returns>Redirect to Swagger UI</returns>
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Index()
    {
        return Redirect("/swagger");
    }
}
