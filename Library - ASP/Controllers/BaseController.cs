using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace Library___ASP.Controllers
{
    public class BaseController : Controller
    {
        protected Guid GetUserID => new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}