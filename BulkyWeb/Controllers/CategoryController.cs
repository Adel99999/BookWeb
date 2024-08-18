using Microsoft.AspNetCore.Mvc;
using BulkyWeb.Data;
using BulkyWeb.Models;
namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var objCategoryList = _db.cateogries.ToList();
            return View(objCategoryList);
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
                _db.cateogries.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Created Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }
            Category obj = _db.cateogries.FirstOrDefault(u=>u.Id==id); 
            if (obj == null) { return  NotFound(); }
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
                _db.cateogries.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Updated Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }
            Category obj = _db.cateogries.FirstOrDefault(u=>u.Id==id); 
            if (obj == null) { return  NotFound(); }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Delete(Category obj)
        {
            if (obj is null) {  return NotFound(); }
            _db.cateogries.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Deleted Successfully";
            return RedirectToAction("Index", "Category");
        }
    }
}
