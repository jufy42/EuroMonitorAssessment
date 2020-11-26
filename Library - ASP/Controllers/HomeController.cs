using System;
using System.Collections.Generic;
using Library___ASP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Library___ASP.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly ILogger _logger;

        public HomeController(IBookService bookService, ILogger<HomeController> logger)
        {
            _bookService = bookService;
            _logger = logger;
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

        public async Task<LibraryBook> GetBook(Guid bookID)
        {
            return await _bookService.GetBookByID(bookID, User.IsInRole(Global.ROLE_ADMINISTRATOR) ? (Guid?)null : GetUserID);
        }

        [HttpPost]
        public async Task<bool> RemoveBook(Guid bookID)
        {
            return await _bookService.RemoveBook(bookID);
        }

        [HttpPost]
        public async Task<string> SaveBookImage(IList<IFormFile> files)
        {
            try
            {
                if (files.Count > 0)
                {
                    var file = files.FirstOrDefault();

                    string extension = Path.GetExtension(file.FileName);

                    if (Global.ImageExtensions.Contains(extension))
                    {
                        var success = await _bookService.SaveBookImage(file);

                        if (success)
                            return "";
                    }
                    else
                    {
                        return "Not an accepted image file format.";    
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"SaveBookImage : {e.Message}");
            }

            return "Something went wrong";
        }
    }
}
