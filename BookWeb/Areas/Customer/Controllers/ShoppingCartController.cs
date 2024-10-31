using BookWeb.DataAccess.Repository.IRepository;
using BookWeb.Models;
using BookWeb.Models.ViewModels;
using BookWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        [BindProperty]
        public ShoppingCartVM obj {  get; set; }
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

			var claims = (ClaimsIdentity)User.Identity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            obj = new()
            {
                shoppingCartList = _UnitOfWork.shoppingCartRepository.GetAll(u => u.ApplicationUserId==userId),
                orderHeader=new()
            };
            foreach(var cart in obj.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQunatity(cart);
                obj.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(obj);
        }
        public IActionResult Summary()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;

            obj = new()
            {
                shoppingCartList = _UnitOfWork.shoppingCartRepository.GetAll(u => u.ApplicationUserId == userId),
                orderHeader = new()
            };

            // Check if the shopping cart is empty
            if (obj.shoppingCartList == null || !obj.shoppingCartList.Any())
            {
                // Redirect to another page if the shopping cart is empty
                return RedirectToAction("Index", "ShoppingCart");
            }

            obj.orderHeader.ApplicationUser = _UnitOfWork.applicationUserRepository.Get(u => u.Id == userId);
            obj.orderHeader.Name = obj.orderHeader.ApplicationUser.Name;
            obj.orderHeader.PhoneNumber = obj.orderHeader.ApplicationUser.PhoneNumber;
            obj.orderHeader.StreetAddress = obj.orderHeader.ApplicationUser.StreetAdress;
            obj.orderHeader.City = obj.orderHeader.ApplicationUser.City;
            obj.orderHeader.State = obj.orderHeader.ApplicationUser.State;
            obj.orderHeader.PostalCode = obj.orderHeader.ApplicationUser.PostalCode;

            foreach (var cart in obj.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQunatity(cart);
                obj.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(obj);
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Retrieve shopping cart for the user
            obj.shoppingCartList = _UnitOfWork.shoppingCartRepository.GetAll(u => u.ApplicationUserId == userId);

            // Create new order
            obj.orderHeader.OrderDate = DateTime.Now;
            obj.orderHeader.ApplicationUserId = userId;
            obj.orderHeader.ApplicationUser = _UnitOfWork.applicationUserRepository.Get(u => u.Id == userId);

            // Calculate order total
            foreach (var cart in obj.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQunatity(cart);
                obj.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            // Set order status
            obj.orderHeader.PaymentStatus = SD.PaymentStatusPending;
            obj.orderHeader.OrderStatus = SD.StatusPending;

            // Save order header
            _UnitOfWork.orderHeaderRepo.Add(obj.orderHeader);
            _UnitOfWork.Save();

            // Save order details
            foreach (var cart in obj.shoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = obj.orderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _UnitOfWork.orderDetailRepo.Add(orderDetail);
                _UnitOfWork.Save();
            }

            // Clear shopping cart after the order is placed
            foreach (var cart in obj.shoppingCartList)
            {
                _UnitOfWork.shoppingCartRepository.Remove(cart);
            }

            // Save changes after removing the shopping cart items
            _UnitOfWork.Save();

            // Redirect to order confirmation page
            return RedirectToAction("OrderConfirmation", new { id = obj.orderHeader.Id });
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
        public IActionResult Remove(int cartId)
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
