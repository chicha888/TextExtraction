using Microsoft.AspNetCore.Mvc;

namespace TextExtraction.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
