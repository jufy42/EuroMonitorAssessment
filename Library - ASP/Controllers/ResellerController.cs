using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Library.ADT;
using Library___ASP.Helpers;
using Microsoft.Extensions.Logging;
using static Newtonsoft.Json.JsonConvert;

namespace Library___ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(BasicFilter))]
    public class ResellerController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;
        private readonly IAccountService _accountService;

        public ResellerController(ILogger<HomeController> logger, IBookService bookService, IAccountService accountService)
        {
            _logger = logger;
            _bookService = bookService;
            _accountService = accountService;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CheckUser(string username, string password)
        {
            _logger.LogInformation($"CheckUser : Username = {username}");
            var userID = await _accountService.ValidateUser(username, password);

            return Ok(SerializeObject(new {Id = userID}));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Subscribe(Guid bookID, Guid userID)
        {
            _logger.LogInformation($"Subscribe : BookID = {bookID}, UserID = {userID}");
            var success = await _bookService.Subscribe(bookID, userID);

            return Ok(SerializeObject(new {Success = success}));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Unsubscribe(Guid bookID, Guid userID)
        {
            _logger.LogInformation($"Unsubscribe : BookID = {bookID}, UserID = {userID}");
            var success = await _bookService.UnSubscribe(bookID, userID);

            return Ok(SerializeObject(new {Success = success}));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllBooks()
        {
            _logger.LogInformation("GetAllBooks");
            var books = await _bookService.GetBooks();

            return Ok(SerializeObject(new {Books = books}));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSubscribedBooks(Guid userID)
        {
            _logger.LogInformation($"GetSubscribedBooks : UserID = {userID}");
            var books = await _bookService.GetBooks(userID);

            return Ok(SerializeObject(new {Books = books.Where(p => p.IsSubscribed == true).ToList()}));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetNonSubscribedBooks(Guid userID)
        {
            _logger.LogInformation($"GetNonSubscribedBooks : UserID = {userID}");
            var books = await _bookService.GetBooks(userID);

            return Ok(SerializeObject(new {Books = books.Where(p => p.IsSubscribed == false).ToList()}));
        }
    }
}
