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
    public class BodyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BodyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Body> objBodyList = _unitOfWork.Body.GetAll();
            return View(objBodyList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Body obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Body.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Body created successfully";
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
            var bodyFromUnitOfWork = _unitOfWork.Body.GetFirstOrDefault(u => u.Id == id);

            if (bodyFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(bodyFromUnitOfWork);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Body obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Body.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Body updated successfully";
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
            var bodyFromUnitOfWork = _unitOfWork.Body.GetFirstOrDefault(u => u.Id == id);

            if (bodyFromUnitOfWork == null)
            {
                return NotFound();
            }

            return View(bodyFromUnitOfWork);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Body.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Body.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Body deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
