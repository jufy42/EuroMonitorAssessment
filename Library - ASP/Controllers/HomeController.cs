using System;
using Library___ASP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Microsoft.AspNetCore.Authorization;

namespace Library___ASP.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IBookService _bookService;

        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> LoadBookList(BookList bookList)
        {
            return PartialView("_BookList", await _bookService.GetBooks(bookList, User.IsInRole(Global.ROLE_ADMINISTRATOR) ? (Guid?)null : GetUserID));
        }

        [HttpPost]
        [Authorize(Roles = Global.ROLE_ADMINISTRATOR)]
        public async Task<bool> SaveBook(LibraryBook book)
        {
            return await _bookService.SaveBook(book);
        }

        [HttpPost]
        public async Task<string> Subscribe(Guid bookID)
        {
            return await _bookService.Subscribe(bookID, GetUserID);
        }

        [HttpPost]
        public async Task<string> Unsubscribe(Guid bookID)
        {
            return await _bookService.UnSubscribe(bookID, GetUserID);
        }
    }
}
