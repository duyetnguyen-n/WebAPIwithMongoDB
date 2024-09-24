using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIwithMongoDB.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Kiểm tra xem người dùng đã đăng nhập và có JWT token hợp lệ chưa
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Bạn cần đăng nhập.");
                return;
            }

            // Lấy thông tin về role từ claims trong token
            var role = context.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            // Kiểm tra quyền hạn dựa trên role
            if (role == "hieu-truong" || role == "admin" || role == "Admin")
            {
                // Hiệu trưởng hoặc admin có quyền truy cập đầy đủ
                await _next(context);
            }
            else if (role == "thay-co" || role == "user" || role == "User")
            {
                // Thầy cô chỉ có quyền truy cập các chức năng hạn chế
                // Bạn có thể kiểm tra thêm permissions hoặc điều kiện khác tại đây
                await _next(context);
            }
            else
            {
                // Người dùng không có quyền
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Bạn không có quyền truy cập vào tài nguyên này.");
            }
        }
    }
}