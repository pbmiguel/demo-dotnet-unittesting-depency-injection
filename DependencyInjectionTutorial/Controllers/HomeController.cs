using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionTutorial.DataAccess;
using DependencyInjectionTutorial.Models;
using Microsoft.AspNetCore.Mvc;
using DependencyInjectionTutorial.Services;

namespace DependencyInjectionTutorial.Controllers
{
    public class HomeController : Controller
    {
        private IBookData _bda;
        private RequestTrackerData _tracker;
        private ICounter _pageCounter;

        public HomeController(IBookData b, RequestTrackerData tr, ICounter pc)
        {
            _bda = b;
            _tracker = tr;
            _pageCounter = pc;
        }

        public ViewResult Index()
        {
            _tracker.Add("Home.Index");
            ViewData["PageCount"] = _pageCounter.Count();
            ViewData["Tracks"] = _tracker.Display();
            return View(_bda.Books);
        }

        public ViewResult Create()
        {
            _tracker.Add("Home.Create.Get");
            ViewData["PageCount"] = _pageCounter.Count();
            ViewData["Tracks"] = _tracker.Display();
            return View(new Book());
        }

        [HttpPost]
        public RedirectToActionResult Create(Book b)
        {
            _tracker.Add("Home.Create.Post");
            _bda.Create(b);
            return RedirectToAction("Index");
        }
    }
}