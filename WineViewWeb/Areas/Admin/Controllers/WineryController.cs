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
    public class WineryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WineryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Winery> objWineryList = _unitOfWork.Winery.GetAll();
            return View(objWineryList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Winery obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Winery.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Winery created successfully";
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
            var wineryFromUnitOfWork = _unitOfWork.Winery.GetFirstOrDefault(u => u.Id == id);

            if (wineryFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(wineryFromUnitOfWork);
        }

        //POST
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Winery obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Winery.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Winery updated successfully";
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
            var wineryFromUnitOfWork = _unitOfWork.Winery.GetFirstOrDefault(u => u.Id == id);

            if (wineryFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(wineryFromUnitOfWork);
        }

        //POST
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Winery.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Winery.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Winery deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
