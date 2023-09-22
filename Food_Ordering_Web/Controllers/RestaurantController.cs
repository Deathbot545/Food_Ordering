using Microsoft.AspNetCore.Mvc;

namespace Food_Ordering_Web.Controllers
{
    public class RestaurantController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Owner/MainPaige.cshtml");
        }

      
        public IActionResult Add()
        {
            return View("~/Views/Owner/AddOutlet.cshtml");
        }

    }
}
