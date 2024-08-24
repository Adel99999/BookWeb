using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess;
using Bulky.Models;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;
namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public ProductController(IUnitOfWork unitofwork)
        {
            _UnitOfWork = unitofwork;
        }

        public IActionResult Index()
        {
            return View(_UnitOfWork.ProductRepository.GetAll());
        }
        public IActionResult Create()
        {

            IEnumerable<SelectListItem> CategoryList = _UnitOfWork.CategoryRepository.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            ProductVM product = new()
            {
                Product = new Product(),
                CategoryList = CategoryList
            };
            return View(product);
        }
        [HttpPost]
        public IActionResult Create(ProductVM obj)
        {
            
            if (ModelState.IsValid)
            {
                _UnitOfWork.ProductRepository.Add(obj.Product);
                _UnitOfWork.Save();
                TempData["success"] = "Created Successfully";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product obj = _UnitOfWork.ProductRepository.Get(u => u.Id == id);
            if (obj == null) { return NotFound(); }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            
            if (ModelState.IsValid)
            {
                _UnitOfWork.ProductRepository.Update(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Updated Successfully";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product obj = _UnitOfWork.ProductRepository.Get(u => u.Id == id);
            if (obj == null) { return NotFound(); }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Delete(Product obj)
        {
            if (obj is null) { return NotFound(); }
            _UnitOfWork.ProductRepository.Remove(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Deleted Successfully";
            return RedirectToAction("Index", "Product");
        }
    }
}
