using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) 
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Villa> data = _unitOfWork.Villa.GetAll();
            return View(data);
        }

        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa villa)
        {
            if (villa.Name == villa.Description)
            {
                ModelState.AddModelError("","Name and description cannot be the same");
            }
            if (ModelState.IsValid)
            {
                if (villa.Image != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImages");
                    string uniqueFileName = Guid.NewGuid().ToString()+ Path.GetExtension(villa.Image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    villa.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    villa.ImageUrl = @"\images\VillaImages\"+uniqueFileName;
                }
                else
                {
                    villa.ImageUrl = "https://placehold.co/600x400?text=Villa";
                }

                _unitOfWork.Villa.Add(villa);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been created successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Update(int villaId)
        {
            Villa? villa = _unitOfWork.Villa.Get(u=>u.Id==villaId);
            if (villa == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villa);
        }

        [HttpPost]
        public IActionResult Update(Villa villa)
        {
            if (ModelState.IsValid)
            {
                if (villa.Image != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImages");
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!string.IsNullOrEmpty(villa.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                            catch (IOException ex)
                            {
                                TempData["error"] = "The image file could not be updated. Please try again.";
                                return View();
                            }
                        }
                    }
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    villa.Image.CopyTo(fileStream);
                    villa.ImageUrl = @"\images\VillaImages\" + uniqueFileName;
                }
                
                _unitOfWork.Villa.Update(villa);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been updated successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int villaId)
        {
            Villa? villa = _unitOfWork.Villa.Get(d => d.Id == villaId);
            if (villa == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villa);
        }

        [HttpPost]
        public IActionResult Delete(Villa villa)
        {
            Villa? obj = _unitOfWork.Villa.Get(d => d.Id == villa.Id);

            if (obj is not null)
            {
                if (!string.IsNullOrEmpty(obj.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        catch (IOException ex)
                        {
                            TempData["error"] = "The image file could not be deleted. Please try again.";
                            return View();
                        }
                    }
                }

                _unitOfWork.Villa.Remove(obj);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "The villa could not be deleted.";
            return View();
        }
    }
}
