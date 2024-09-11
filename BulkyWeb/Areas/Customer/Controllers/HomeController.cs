using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
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
            IEnumerable<Product> productList= _UnitOfWork.ProductRepository.GetAll();
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
            obj.ApplicationUserId = userId;
            obj.Id = 0;
            // here we get the طلبيه  that exist in the table shoppingCart in DB
            ShoppingCart cart = _UnitOfWork.shoppingCartRepository.Get(u => u.ApplicationUserId == userId && u.ProductId == obj.ProductId);
            if (cart != null) // if the طلبيه  exist then we should update in it
            {
                cart.Count += obj.Count;
                _UnitOfWork.shoppingCartRepository.Update(cart);
            }
            else // if the order not exist then we should make new one (new one with primary key)
            {
                _UnitOfWork.shoppingCartRepository.Add(obj);
            }
            TempData["success"] = "Cart Updated successfully";
            _UnitOfWork.Save();
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
