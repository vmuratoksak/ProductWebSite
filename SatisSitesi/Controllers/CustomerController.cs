using Microsoft.AspNetCore.Mvc;
using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;

namespace SatisSitesi.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult Index(string search, string role = "All", string status = "Active", int page = 1)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userEmail) || userRole != "Admin")
                return RedirectToAction("Login", "Auth");

            var model = _customerService.GetPaged(search, role, status, page, 8);
            return View(model);
        }

        public IActionResult Edit(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return RedirectToAction("Login", "Auth");

            var item = _customerService.GetById(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(UserEntity model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return RedirectToAction("Login", "Auth");

            // We basicly only want to allow editing Username, Email and Role for now
            _customerService.Update(model);
            TempData["Success"] = "Müşteri bilgileri güncellendi.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return RedirectToAction("Login", "Auth");

            _customerService.Delete(id);
            TempData["Info"] = "Müşteri kaydı silindi.";
            return RedirectToAction("Index");
        }

        public IActionResult Add()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return RedirectToAction("Login", "Auth");

            return View(new UserEntity { Role = "User" });
        }

        [HttpPost]
        public IActionResult Add(UserEntity model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return RedirectToAction("Login", "Auth");

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email))
            {
                TempData["Error"] = "Kullanıcı adı ve Email zorunludur.";
                return View(model);
            }

            _customerService.Add(model);
            TempData["Success"] = "Yeni müşteri başarıyla eklendi.";
            return RedirectToAction("Index");
        }
    }
}
