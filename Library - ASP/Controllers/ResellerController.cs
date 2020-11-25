using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Library.ADT;
using Library.Core;
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

        /// <summary>
        /// Gets the ID of the user with the details specified
        /// </summary>
        /// <param name="username">The username or email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CheckUser(string username, string password)
        {
            _logger.LogInformation($"CheckUser : Username = {username}");
            var userID = await _accountService.ValidateUser(username, password);

            return Ok(SerializeObject(new {Id = userID}));
        }

        /// <summary>
        /// Registers the user on the system and return the Id of the new user
        /// </summary>
        /// <param name="model">Contains the details for registration.</param>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegisterUser(RegisterViewModel model)
        {
            _logger.LogInformation($"RegisterUser : model = {SerializeObject(model)}");
            var success = await _accountService.RegisterUser(model);

            if (success)
                return Ok(SerializeObject(new {Id = await _accountService.ValidateUser(model.Email, model.Password)}));

            return Ok(SerializeObject(new {Id = (Guid?)null}));
        }

        /// <summary>
        /// Subscribe to a book
        /// </summary>
        /// <param name="bookID">The Id of the book.</param>
        /// <param name="userID">The Id of the user.</param>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Subscribe(Guid bookID, Guid userID)
        {
            _logger.LogInformation($"Subscribe : BookID = {bookID}, UserID = {userID}");
            var success = await _bookService.Subscribe(bookID, userID);

            return Ok(SerializeObject(new {Success = success}));
        }

        /// <summary>
        /// Unsubscribe to a book
        /// </summary>
        /// <param name="bookID">The Id of the book.</param>
        /// <param name="userID">The Id of the user.</param>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Unsubscribe(Guid bookID, Guid userID)
        {
            _logger.LogInformation($"Unsubscribe : BookID = {bookID}, UserID = {userID}");
            var success = await _bookService.UnSubscribe(bookID, userID);

            return Ok(SerializeObject(new {Success = success}));
        }

        /// <summary>
        /// Get all the books in the store
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllBooks()
        {
            _logger.LogInformation("GetAllBooks");
            var books = await _bookService.GetBooks();

            return Ok(SerializeObject(new {Books = books}));
        }

        /// <summary>
        /// Get all the books the user is subscribed to
        /// </summary>
        /// <param name="userID">The Id of the user.</param>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSubscribedBooks(Guid userID)
        {
            _logger.LogInformation($"GetSubscribedBooks : UserID = {userID}");
            var books = await _bookService.GetBooks(userID);

            return Ok(SerializeObject(new {Books = books.Where(p => p.IsSubscribed == true).ToList()}));
        }

        /// <summary>
        /// Get all the books the user is not subscribed to
        /// </summary>
        /// <param name="userID">The Id of the user.</param>
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
