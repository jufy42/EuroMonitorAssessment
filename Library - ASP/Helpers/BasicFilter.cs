using Microsoft.AspNetCore.Builder;

namespace Library___ASP.Helpers
{
    public class BasicFilter
    {
        public void Configure(IApplicationBuilder appBuilder) {
            appBuilder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}