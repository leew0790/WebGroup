﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectComp1640.Data;
using ProjectComp1640.Dtos.Blog;
using ProjectComp1640.Model;
using System.Security.Claims;

namespace ProjectComp1640.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public BlogController(ApplicationDBContext context, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // -------------------------- CREATE --------------------------
        [Authorize (Roles = "Student,Tutor")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBlog([FromForm] BlogDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            string fileUrl = null;
            if (dto.File != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }
                fileUrl = "/uploads/" + fileName;
            }

            var blog = new Blog
            {
                UserId = userId,
                Title = dto.Title,
                Content = dto.Content,
                Url = fileUrl,

                CreatedAt = DateTime.UtcNow
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Blog created successfully." });
        }
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Comments)
                .ThenInclude(c => c.User)
                .Select(b => new
                {
                    b.Id,
                    b.Title,  
                    b.Content,
                    b.Url,
                    b.CreatedAt,
                    User = b.User.UserName,
                    Comments = b.Comments.Select(c => new
                    {
                        c.Id,
                        c.Content,
                        c.CreatedOn,
                        User = c.User != null ? c.User.FullName : "Unknown User"
                    }).ToList()
                }).ToListAsync();
            return Ok(blogs);
        }
        [Authorize]
        [HttpGet("blog/{id}")]
        public async Task<IActionResult> GetBlogsById(int id)
        {
            var blog = await _context.Blogs
                .Where(b => b.Id == id)
                .Include(b => b.User)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Content,
                    b.Url,
                    b.CreatedAt,
                    User = b.User != null ? b.User.UserName : "Unknown User"
                })
                .FirstOrDefaultAsync();

            if (blog == null)
            {
                return BadRequest("No blog");
            }

            return Ok(blog);
        }

        // -------------------------- UPDATE BLOG --------------------------
        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBlog(int id, [FromForm] BlogDto dto)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) return NotFound();

            // Kiểm tra quyền sở hữu bài viết
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (blog.UserId != userId) return Forbid();

            blog.Title = dto.Title;
            blog.Content = dto.Content;

            if (dto.File != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }
                blog.Url = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Blog updated, pending approval again." });
        }

        // -------------------------- DELETE BLOG --------------------------
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            // Load blog kèm theo các comment liên quan
            var blog = await _context.Blogs
                            .Include(b => b.Comments)
                            .FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (blog.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            // Xóa các comment liên quan trước
            if (blog.Comments != null && blog.Comments.Any())
            {
                _context.Comments.RemoveRange(blog.Comments);
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Blog deleted." });
        }
    }
}
