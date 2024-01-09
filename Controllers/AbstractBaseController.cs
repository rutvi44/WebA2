using Microsoft.AspNetCore.Mvc;

namespace Ass2RM.Controllers
{
	public class AbstractBaseController : Controller
	{
		public void SetWelcome()
		{
			const string cookieName = "firstVisitDate";

			var firstVisitDate = HttpContext.Request.Cookies.ContainsKey(cookieName) &&
				DateTime.TryParse(HttpContext.Request.Cookies[cookieName], out var parsedDate)
				? parsedDate : DateTime.Now;

			var welcomeMessage = HttpContext.Request.Cookies.ContainsKey(cookieName)
				? $"Welcome Back! You first used this app on {firstVisitDate.ToShortDateString()}"
				: "Hey, Welcom to the Course Manager App!";

			var cookieOptions = new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30),
			};

			HttpContext.Response.Cookies.Append(cookieName, firstVisitDate.ToString(), cookieOptions);

			ViewData["WelcomeMessage"] = welcomeMessage;
		}
	}
}
