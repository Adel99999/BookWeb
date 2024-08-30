using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess;
using Bulky.Models;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Bulky.Utility;
namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CategoryController(IUnitOfWork unitofwork)
        {
            _UnitOfWork = unitofwork;
        }

        public IActionResult Index()
        {
            return View(_UnitOfWork.CategoryRepository.GetAll());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "You can't put the Category and DisplayOrder the same.");
            }

            if (obj.Name.Any(char.IsDigit))
            {
                ModelState.AddModelError("Name", "The Name field cannot contain numbers.");
            }

            if (!obj.DisplayOrder.ToString().All(char.IsDigit))
            {
                ModelState.AddModelError("DisplayOrder", "The DisplayOrder field can only contain numbers.");
            }
            if (ModelState.IsValid)
            {
                _UnitOfWork.CategoryRepository.Add(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Created Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category obj = _UnitOfWork.CategoryRepository.Get(u => u.Id == id);
            if (obj == null) { return NotFound(); }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "You can't put the Category and DisplayOrder the same.");
            }

            if (obj.Name.Any(char.IsDigit))
            {
                ModelState.AddModelError("Name", "The Name field cannot contain numbers.");
            }

            if (!obj.DisplayOrder.ToString().All(char.IsDigit))
            {
                ModelState.AddModelError("DisplayOrder", "The DisplayOrder field can only contain numbers.");
            }
            if (ModelState.IsValid)
            {
                _UnitOfWork.CategoryRepository.Update(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Updated Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category obj = _UnitOfWork.CategoryRepository.Get(u => u.Id == id);
            if (obj == null) { return NotFound(); }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Delete(Category obj)
        {
            if (obj is null) { return NotFound(); }
            _UnitOfWork.CategoryRepository.Remove(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Deleted Successfully";
            return RedirectToAction("Index", "Category");
        }
    }
}
