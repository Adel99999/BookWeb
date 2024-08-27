using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess;
using Bulky.Models;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitofwork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = unitofwork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View(_UnitOfWork.ProductRepository.GetAll());
        }
        public IActionResult Upsert(int? id)
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
            if (id == 0 || id == null)
            {
                return View(product);
            }
            else
            {
                product.Product = _UnitOfWork.ProductRepository.Get(x => x.Id == id);
                return View(product);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        var oldimage = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimage))
                        {
                            System.IO.File.Delete(oldimage);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    obj.Product.ImageUrl = @"\images\product\" + filename;
                }
                if (obj.Product.Id == 0) // create
                {
                    _UnitOfWork.ProductRepository.Add(obj.Product);
                }
                else // update
                {
                    _UnitOfWork.ProductRepository.Update(obj.Product);
                }
                _UnitOfWork.Save();
                TempData["success"] = "Operation Successful";
                return RedirectToAction("Index", "Product");
            }
            // If the model state is not valid, return the view with the same model to show errors.
            return View(obj);
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
