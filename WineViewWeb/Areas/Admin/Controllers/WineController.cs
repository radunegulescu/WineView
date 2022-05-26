using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WineView.DataAccess;
using WineView.DataAccess.Repository.IRepository;
using WineView.Models;
using WineView.Models.ViewModels;
using WineView.Utility;

namespace WineViewWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Producer)]
    public class WineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;


        public WineController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            WineVM wineVM = new()
            {
                Wine = new(),
                StyleList = _unitOfWork.Style.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                ),
                ColorList = _unitOfWork.Color.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                ),
                WineryList = _unitOfWork.Winery.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                ),
                GrapeList = _unitOfWork.Grape.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                )
            };
            if (id == null || id == 0)
            {
                //create Wine
                return View(wineVM);
            }
            else
            {
                if (User.IsInRole(SD.Role_Admin))
                {
                    //update Wine
                    wineVM.Wine = _unitOfWork.Wine.GetFirstOrDefault(u => u.Id == id, includeProperties: "Grapes");
                    wineVM.GrapeList = _unitOfWork.Grape.GetAll().Select(
                                        u => new SelectListItem
                                        {
                                            Text = u.Name,
                                            Value = u.Id.ToString(),
                                            Selected = wineVM.Wine.Grapes.Select(g => g.Id).Contains(u.Id)
                                        });
                    return View(wineVM);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(WineVM obj, IFormFile? file, string[]? grapes)
        {
            var wineWithSameClasifierId = _unitOfWork.Wine.GetFirstOrDefault(
                u => u.ClasifierId == obj.Wine.ClasifierId
                && u.Id != obj.Wine.Id
                && u.IsInClasifier);
            if (obj.Wine.IsInClasifier && wineWithSameClasifierId != null)
            {
                ModelState.AddModelError("CustomError", "The Clasifier Id already exists.");
            }
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\wines");
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.Wine.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Wine.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Wine.ImageUrl = @"\images\wines\" + fileName + extension;
                }
                if (grapes != null)
                {
                    var items = _unitOfWork.Grape.GetAll().Select(
                                u => new SelectListItem
                                {
                                    Text = u.Name,
                                    Value = u.Id.ToString()
                                }
                                );
                    obj.Wine.Grapes = new();
                    foreach (SelectListItem item in items)
                    {
                        if (grapes.Contains(item.Value))
                        {
                            Grape grapeFromDb = _unitOfWork.Grape.GetFirstOrDefault(u => u.Id.ToString() == item.Value);
                            obj.Wine.Grapes.Add(grapeFromDb);
                        }
                    }
                }
                if (obj.Wine.Id == 0)
                {
                    _unitOfWork.Wine.Add(obj.Wine);
                    TempData["success"] = "Wine created successfully";
                }
                else
                {
                    if (User.IsInRole(SD.Role_Admin))
                    {
                        Wine wineFromDb = _unitOfWork.Wine.GetFirstOrDefault(u => u.Id == obj.Wine.Id, includeProperties: "Grapes");
                        wineFromDb.Grapes.RemoveAll(u => true);
                        _unitOfWork.Wine.Update(obj.Wine);
                        TempData["success"] = "Wine updated successfully";
                    }
                }
                _unitOfWork.Save();
                var wines = _unitOfWork.Wine.GetAll();

                return RedirectToAction("Index");
            }
            obj.StyleList = _unitOfWork.Style.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                );
            obj.ColorList = _unitOfWork.Color.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                );
            obj.WineryList = _unitOfWork.Winery.GetAll().Select(
            u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }
            );
            obj.GrapeList = _unitOfWork.Grape.GetAll().Select(
            u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),
                Selected = grapes.Contains(u.Id.ToString())
            });
            return View(obj);
        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var wineList = _unitOfWork.Wine.GetAll(includeProperties: "Winery,Color,Style,Grapes");
            return Json(new { data = wineList });
        }

        //POST
        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Wine.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Wine.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
