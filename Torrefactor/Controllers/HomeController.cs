using System.Web.Mvc;

namespace Torrefactor.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Admin()
		{
			return View();
		}
	}
}