using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Food_Ordering_Web.Controllers
{
    [Authorize(Roles = "Restaurant")]
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

        public IActionResult Manage()
        {
            return View("~/Views/Owner/Manage.cshtml");
        }
       
    }
}
