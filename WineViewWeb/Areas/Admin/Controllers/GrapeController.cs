using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineView.DataAccess;
using WineView.DataAccess.Repository.IRepository;
using WineView.Models;
using WineView.Utility;

namespace WineViewWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Producer)]
    public class GrapeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public GrapeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Grape> objGrapeList = _unitOfWork.Grape.GetAll();
            return View(objGrapeList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Grape obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Grape.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Grape created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var grapeFromUnitOfWork = _unitOfWork.Grape.GetFirstOrDefault(u => u.Id == id);

            if (grapeFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(grapeFromUnitOfWork);
        }

        //POST
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Grape obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Grape.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Grape updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var grapeFromUnitOfWork = _unitOfWork.Grape.GetFirstOrDefault(u => u.Id == id);

            if (grapeFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(grapeFromUnitOfWork);
        }

        //POST
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Grape.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Grape.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Grape deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
