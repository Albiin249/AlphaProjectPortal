using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaPortal.Controllers;

[Authorize]
public class OverviewController : Controller
{
    [Route("alpha/overview")]
    public IActionResult Index()
    {
        return View();
    }
}
