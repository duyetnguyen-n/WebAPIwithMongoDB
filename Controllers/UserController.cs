using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Interface;
using WebAPIwithMongoDB.Services;
using Microsoft.AspNetCore.Hosting;

namespace WebAPIwithMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _users;
        private readonly ITeachGroupRepository _teachGroup;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public UserController(IUserRepository users, ITeachGroupRepository teachGroup, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _users = users;
            _env = webHostEnvironment;
            _configuration = configuration;
            _teachGroup = teachGroup;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<User>>>> GetUsers()
        {
            var Users = await _users.GetAsync();
            return Ok(new ApiResponse<IEnumerable<User>>(200, "Thành công", Users));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<User>>> GetUser(string id)
        {
            if (!await _users.Exists(id))
                return NotFound(new ApiResponse<User>(404, "Không tìm thấy nhóm tiêu chí", null));

            var user = await _users.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<User>(200, "Thành công", user));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<User>>> PostUser([FromForm] User user, IFormFile avatar)
        {
            try
            {
                if (user == null || !ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<User>(400, "Dữ liệu người dùng không hợp lệ", null));
                }

                // Xử lý upload file avatar
                string avatarFileName = null;
                if (avatar != null)
                {
                    var uploadResponse = await UploadFile(avatar);
                    if (uploadResponse.Result is BadRequestObjectResult)
                    {
                        return BadRequest(new ApiResponse<User>(400, "File không hợp lệ", null));
                    }
                    var uploadResult = (ApiResponse<string>)((ObjectResult)uploadResponse.Result).Value;
                    avatarFileName = uploadResult.Data;
                }

                var newUser = new User
                {
                    NumberPhone = user.NumberPhone,
                    Password = user.Password,
                    Position = user.Position,
                    Name = user.Name,
                    Age = user.Age,
                    Mail = user.Mail,
                    Gender = user.Gender,
                    TeachGroupId = user.TeachGroupId,
                    Point = user.Point,
                    Avatar = avatarFileName, // Lưu tên file vào thuộc tính Avatar
                };

                await _users.CreateAsync(newUser);

                return Ok(new ApiResponse<User>(200, "Thêm người dùng thành công", newUser));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} - StackTrace: {ex.StackTrace}");
                return StatusCode(500, new ApiResponse<User>(500, "Lỗi hệ thống. Vui lòng thử lại sau.", null));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<User>>> PutUser([FromForm] User user, IFormFile avatar)
        {
            if (!await _users.Exists(user.Id))
                return NotFound(new ApiResponse<User>(404, "Không tìm thấy người dùng", null));

            var userOld = await _users.GetAsync(user.Id);

            if (userOld.TeachGroupId != user.TeachGroupId)
            {
                await  _teachGroup.DecrementTeachGroupCount(userOld.TeachGroupId);
                await _teachGroup.IncrementTeachGroupCount(user.TeachGroupId);
            }

            userOld.NumberPhone = user.NumberPhone;
            userOld.Password = user.Password;
            userOld.Position = user.Position;
            userOld.Name = user.Name;
            userOld.Age = user.Age;
            userOld.Mail = user.Mail;
            userOld.Gender = user.Gender;
            userOld.TeachGroupId = user.TeachGroupId; 
            userOld.Point = user.Point;

            // Kiểm tra nếu có ảnh đại diện mới
            if (avatar != null)
            {
                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(userOld.Avatar))
                {
                    var oldAvatarPath = Path.Combine(_env.WebRootPath, "uploads", userOld.Avatar);
                    if (System.IO.File.Exists(oldAvatarPath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldAvatarPath);
                            Console.WriteLine($"Deleted old avatar: {oldAvatarPath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to delete old avatar: {ex.Message}");
                        }
                    }
                }

                // Upload file avatar mới
                var uploadResponse = await UploadFile(avatar);
                if (uploadResponse.Result is BadRequestObjectResult)
                {
                    return BadRequest(new ApiResponse<User>(400, "File không hợp lệ", null));
                }
                var uploadResult = (ApiResponse<string>)((ObjectResult)uploadResponse.Result).Value;
                userOld.Avatar = uploadResult.Data; // Cập nhật ảnh đại diện mới
            }

            await _users.UpdateAsync(user.Id, userOld);

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<User>(400, "Lỗi chưa thêm được dữ liệu đâu", null));

            return Ok(new ApiResponse<User>(200, "Cập nhật thành công", userOld));
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUser(string id)
        {
            if (!await _users.Exists(id))
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy nhóm tiêu chí", null));

            var user = await _users.GetAsync(id);

            if (user != null && !string.IsNullOrEmpty(user.Avatar))
            {
                var avatarPath = Path.Combine(_env.WebRootPath, "uploads", user.Avatar);
                Console.WriteLine($"Attempting to delete file at: {avatarPath}");

                if (System.IO.File.Exists(avatarPath))
                {
                    try
                    {
                        System.IO.File.Delete(avatarPath);
                        Console.WriteLine("File deleted successfully.");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Unauthorized access: {ex.Message}");
                        return StatusCode(403, new ApiResponse<string>(403, "Không có quyền xóa ảnh đại diện", null));
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"File in use or IO error: {ex.Message}");
                        return StatusCode(500, new ApiResponse<string>(500, "Ảnh đang được sử dụng bởi một ứng dụng khác hoặc gặp lỗi IO", null));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"General error deleting file: {ex.Message}");
                        return StatusCode(500, new ApiResponse<string>(500, "Lỗi khi xóa ảnh đại diện", null));
                    }
                }
                else
                {
                    Console.WriteLine("File does not exist.");
                }
            }
            await _users.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }


        [Route("UploadFile")]
        [HttpPost]
        // Hàm xử lý upload file và trả về tên file
        private async Task<ActionResult<ApiResponse<string>>> UploadFile(IFormFile file)
        {
            try
            {
                // Kiểm tra phần mở rộng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    return BadRequest(new ApiResponse<string>(400, "File không hợp lệ", null));
                }

                // Tạo thư mục nếu chưa tồn tại trong wwwroot/uploads
                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Tạo tên file duy nhất
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadFolder, fileName);

                // Lưu file vào thư mục uploads
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new ApiResponse<string>(200, "Thêm thành công", fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} - StackTrace: {ex.StackTrace}");
                return BadRequest(new ApiResponse<string>(500, "Đã có lỗi xảy ra", null));
            }
        }

    }
}
