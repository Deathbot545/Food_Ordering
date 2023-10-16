using Microsoft.AspNetCore.Mvc;

namespace Food_Ordering_Web.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Menu(int outletId, int tableId)
        {
            // You can pass these IDs to your view if needed
            ViewBag.OutletId = outletId;
            ViewBag.TableId = tableId;

            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Code to fetch data that only authenticated users should see
                // Populate a model or a ViewBag/ViewData if necessary

                // Return a view that is specific to authenticated users
                return View("~/Views/Menu/AuthenticatedMenu.cshtml");
            }
            else
            {
                // Code to fetch data that guests should see
                // Populate a model or a ViewBag/ViewData if necessary

                // Return a view that is specific to guests
                return View("~/Views/Menu/GuestMenu.cshtml");
            }
        }

    }
}
