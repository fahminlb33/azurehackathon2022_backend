using Microsoft.AspNetCore.Http;

namespace Evangelion01.BackendApp.Infrastructure
{
    public static class ExtensionMethods
    {
        public static string GetStudentId(this HttpContext context)
        {
            var claim = context.User.FindFirst("studentId");
            return claim.Value;
        }
    }
}
