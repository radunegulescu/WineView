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
    public class StyleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StyleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Style> objStyleList = _unitOfWork.Style.GetAll();
            return View(objStyleList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Style obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Style.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Style created successfully";
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
            var styleFromUnitOfWork = _unitOfWork.Style.GetFirstOrDefault(u => u.Id == id);

            if (styleFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(styleFromUnitOfWork);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Style obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Style.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Style updated successfully";
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
            var styleFromUnitOfWork = _unitOfWork.Style.GetFirstOrDefault(u => u.Id == id);

            if (styleFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(styleFromUnitOfWork);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Style.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Style.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Style deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
