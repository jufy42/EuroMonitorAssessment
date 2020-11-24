using System;
using System.Text;
using System.Threading.Tasks;
using Library.ADT;
using Microsoft.AspNetCore.Http;

namespace Library___ASP.Helpers
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAccountService _accountService;

        public AuthenticationMiddleware(RequestDelegate next, IAccountService accountService)
        {
            _next = next;
            _accountService = accountService;
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);

                var verification = await _accountService.ValidateUser(username, password);
                if (verification == null)
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = 401;
                }
            }
            else
            {
                context.Response.StatusCode = 401;
            }
        }
    }
}
