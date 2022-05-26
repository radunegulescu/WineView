using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WineView.DataAccess.Repository.IRepository;
using WineView.Models;
using WineView.Models.ViewModels;
using WineView.Utility;

namespace WineViewWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index(string? searchedWine)
        {
            IEnumerable<Wine> wineList = _unitOfWork.Wine.GetAll(includeProperties: "Winery,Color,Style,Grapes");
            if (searchedWine != null)
            {
                wineList = wineList.Where(u => u.Name.ToUpper().Contains(searchedWine.ToUpper()) ||
                                               u.Winery.Name.ToUpper().Contains(searchedWine.ToUpper()) ||
                                               u.Style.Name.ToUpper().Contains(searchedWine.ToUpper()) ||
                                               u.Color.Name.ToUpper().Contains(searchedWine.ToUpper()));
            }
            ViewBag.SearchedWine = searchedWine;
            return View(wineList);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPOST(string searchedWine)
        {
            return RedirectToAction("Index", "Home", new { searchedWine = searchedWine });
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_User)]
        public IActionResult Details(int wineId)
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                WineId = wineId,
                Wine = _unitOfWork.Wine.GetFirstOrDefault(u => u.Id == wineId, includeProperties: "Winery,Color,Style,Grapes")
            };
            var reviews = _unitOfWork.Review.GetAll(u => u.WineId == wineId, includeProperties: "Body");
            if (reviews.Any())
            {
                ViewBag.Sweetness = (double)reviews.Select(u => u.Sweetness).Sum() / (double)reviews.Count();
                ViewBag.Acidity = (double)reviews.Select(u => u.Acidity).Sum() / (double)reviews.Count();
                ViewBag.Tannin = (double)reviews.Select(u => u.Tannin).Sum() / (double)reviews.Count();
                ViewBag.Body = reviews.Select(u => u.Body).GroupBy(s => s)
                             .OrderByDescending(s => s.Count())
                             .First().Key;
            }

            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_User)]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value && u.WineId == shoppingCart.WineId);

            if (cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);

            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
                _unitOfWork.Save();

            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_User)]
        public IActionResult Classify()
        {
            IEnumerable<Wine> wineList = _unitOfWork.Wine.GetAll(u => u.IsInClasifier, includeProperties: "Winery,Color,Style,Grapes");
            return View(wineList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_User)]
        public IActionResult Classify(IFormFile? file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\temp");
                var extension = Path.GetExtension(file.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                var url = @"\images\temp\" + fileName + extension;
                var imgPath = Path.Combine(wwwRootPath, url.TrimStart('\\'));

                var psi = new ProcessStartInfo();
                psi.FileName = @"C:\ProgramData\Anaconda3\envs\rl-2\python.exe";
                var script = @"D:\Radu\IA\pythonProject1\Scripturi\test.py";


                psi.Arguments = script + " " + imgPath;

                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                var errors = "";
                var results = "";

                using (var process = Process.Start(psi))
                {
                    errors = process.StandardError.ReadToEnd();
                    results = process.StandardOutput.ReadToEnd();
                }
                int wineClasifierId = Int16.Parse(results);
                System.IO.File.Delete(imgPath);


                if (wineClasifierId != -1)
                {
                    var idFromDb = _unitOfWork.Wine.GetFirstOrDefault(u => u.IsInClasifier && u.ClasifierId == wineClasifierId).Id;
                    return RedirectToAction("Details", "Home", new { wineId = idFromDb });
                }
                else
                {
                    TempData["error"] = "No Wine Found!";
                    return RedirectToAction("Classify");
                }



            }
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}