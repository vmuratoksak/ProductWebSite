using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        private static List<string> Names = new List<string>();

        // GET
        [HttpGet]
        public IActionResult Index(int? editIndex)
        {
            var model = new HelloViewModel
            {
                Message = TempData["Message"] as string ?? "Lütfen adınızı girin",
                Date = DateTime.Now,
                Names = Names
            };

            // UPDATE için formu doldur
            if (editIndex != null && editIndex >= 0 && editIndex < Names.Count)
            {
                model.Name = Names[editIndex.Value];
                model.EditIndex = editIndex;
                model.Message = "İsmi güncelle";
            }

            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(HelloViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Names = Names;
                model.Date = DateTime.Now;
                return View(model);
            }

            if (model.EditIndex != null)
            {
                // UPDATE
                Names[model.EditIndex.Value] = model.Name;
                TempData["Message"] = "İsim güncellendi ✅";
            }
            else
            {
                // CREATE
                Names.Add(model.Name);
                TempData["Message"] = "İsim eklendi ✅";
            }

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int index)
        {
            if (index >= 0 && index < Names.Count)
            {
                Names.RemoveAt(index);
            }

            return RedirectToAction("Index");
        }
    }
}
