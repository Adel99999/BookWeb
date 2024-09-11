using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM obj = new()
            {
                shoppingCartList = _UnitOfWork.shoppingCartRepository.GetAll(u => u.ApplicationUserId==userId)

            };
            foreach(var cart in obj.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQunatity(cart);
                obj.OrderTotal += (cart.Price * cart.Count);
            }
            return View(obj);
        }
        public IActionResult Summary()
        {
            return View();
        }
        public IActionResult Plus(int cartId)
        {
            var cartDb = _UnitOfWork.shoppingCartRepository.Get(u => u.Id == cartId);
            cartDb.Count += 1;
            _UnitOfWork.shoppingCartRepository.Update(cartDb);
            _UnitOfWork.Save();
            return RedirectToAction("Index");
        } 
        public IActionResult Minus(int cartId)
        {
            var cartDb = _UnitOfWork.shoppingCartRepository.Get(u => u.Id == cartId);
            if (cartDb.Count <= 1)
            {
                _UnitOfWork.shoppingCartRepository.Remove(cartDb);
            }
            else
            {
                cartDb.Count -= 1;
                _UnitOfWork.shoppingCartRepository.Update(cartDb);
            }
            _UnitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Reomve(int cartId)
        {
            var cartDb = _UnitOfWork.shoppingCartRepository.Get(u => u.Id == cartId);
            _UnitOfWork.shoppingCartRepository.Remove(cartDb);
            _UnitOfWork.Save();
            return RedirectToAction("Index");
        }
        private double GetPriceBasedOnQunatity(ShoppingCart obj)
        {
            if (obj.Count <= 50)
            {
                return obj.product.Price;
            }
            else
            {
                if (obj.Count <= 100)
                {
                    return obj.product.Price50;
                }
                else
                {
                    return obj.product.Price100;
                }
            }
        }
    }
}
