using BookWeb.DataAccess.Repository.IRepository;
using BookWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
	{
		private readonly IUnitOfWork _UnitOfWork;
		public HomeController(IUnitOfWork unitOfWork)
		{
			_UnitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			IEnumerable<Product> productList = _UnitOfWork.ProductRepository.GetAll();
			return View(productList);
		}

		public IActionResult Details(int id)
		{
			var product = _UnitOfWork.ProductRepository.Get(u => u.Id == id);
			var cart = new ShoppingCart
			{
				product = product,
				Count = 1,
				ProductId = id
			};
			return View(cart);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Details(ShoppingCart obj)
		{
			var claims = (ClaimsIdentity)User.Identity;
			var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
			obj.ApplicationUserId = userId; // Associate with the current logged-in user
			obj.Id = 0; // Ensure this is a new entry

			// Retrieve existing cart item for the current user and product
			ShoppingCart cart = _UnitOfWork.shoppingCartRepository.Get(u => u.ApplicationUserId == userId && u.ProductId == obj.ProductId);

			if (cart != null) // If it exists, update it
			{
				cart.Count += obj.Count;
				_UnitOfWork.shoppingCartRepository.Update(cart);
			}
			else // If it doesn't exist, add a new item
			{
				_UnitOfWork.shoppingCartRepository.Add(obj);
			}

			_UnitOfWork.Save();
			TempData["success"] = "Cart updated successfully";
			return RedirectToAction("Index");
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
