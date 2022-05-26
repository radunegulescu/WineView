using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineView.DataAccess;
using WineView.DataAccess.Repository.IRepository;
using WineView.Models;
using WineView.Utility;

namespace WineViewWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ColorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ColorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Color> objColorList = _unitOfWork.Color.GetAll();
            return View(objColorList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Color obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Color.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Color created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var colorFromUnitOfWork = _unitOfWork.Color.GetFirstOrDefault(u => u.Id == id);

            if (colorFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(colorFromUnitOfWork);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Color obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Color.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Color updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var colorFromUnitOfWork = _unitOfWork.Color.GetFirstOrDefault(u => u.Id == id);

            if (colorFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(colorFromUnitOfWork);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Color.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Color.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Color deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
