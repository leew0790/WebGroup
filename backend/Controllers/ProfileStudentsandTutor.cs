﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectComp1640.Data;
using ProjectComp1640.Dtos.Account;
using ProjectComp1640.Model;
using System.Linq;
using System.Security.Claims;

namespace ProjectComp1640.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileStudentsandTutorController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public ProfileStudentsandTutorController(ApplicationDBContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            //if(_context.Students.Any())
            //{
                var students = await _context.Students
                .Include(s => s.User)
                .Select(s => new {
                    s.Id,
                    s.StudentCode,
                    s.Course,
                    s.Status,
                    User = s.User != null ? new { s.User.FullName, s.User.UserName, s.User.Email } : null
                })
                .ToListAsync();

                return Ok(students);

        //}
            //return BadRequest("Student not found");
        }
        [Authorize] 
        [HttpGet("students/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            // Lấy userId từ token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            // Truy vấn sinh viên theo ID
            var studentQuery = _context.Students
                .Include(s => s.User)
                .Where(s => s.Id == id);

            var student = await studentQuery
                .Select(s => new
                {
                    s.Id,
                    s.StudentCode,
                    s.Course,
                    s.Status,
                    User = new
                    {
                        s.User.FullName,
                        s.User.UserName,
                        s.User.Email
                    }
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return Forbid(); // Không có quyền xem thông tin
            }

            return Ok(student);
        }


        [Authorize( Roles = "Admin")]
        [HttpGet("tutors")]
        public async Task<IActionResult> GetTutors()
        {

            if (_context.Tutors.Any())
            {
                var tutors = await _context.Tutors
                .Include(t => t.User)
                .Select(t => new {
                    t.Id,
                    t.Department,
                    t.ExperienceYears,
                    t.Rating,
                    User = t.User != null ? new { t.User.FullName, t.User.UserName, t.User.Email } : null
                })
                .ToListAsync();
                return Ok(tutors);
            }
               return BadRequest("Tutor not found");

          
        }
        [Authorize] // 👈 Cho phép cả Admin và Tutor truy cập
        [HttpGet("tutors/{id}")]
        public async Task<IActionResult> GetTutorById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            
            // Truy vấn giảng viên theo ID
            var tutorQuery = _context.Tutors
                .Include(t => t.User)
                .Where(t => t.Id == id);

           

            var tutor = await tutorQuery
                .Select(t => new
                {
                    t.Id,
                    t.Department,
                    t.ExperienceYears,
                    t.Rating,
                    User = new
                    {
                        t.User.FullName,
                        t.User.UserName,
                        t.User.Email
                    }
                })
                .FirstOrDefaultAsync();

            if (tutor == null)
            {
                return Forbid(); // Không có quyền xem thông tin
            }

            return Ok(tutor);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-student/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] RegisterStudentDto registerDto)
        {
            if (registerDto == null) return BadRequest("Invalid data!");

            var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound("Student not found!");

            student.StudentCode = registerDto.StudentCode;
            student.Course = registerDto.Course;
            student.Status = registerDto.Status;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Student updated successfully!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-tutor/{id}")]
        public async Task<IActionResult> UpdateTutor(int id, [FromBody] RegisterTutorDto registerDto)
        {
            if (registerDto == null) return BadRequest("Invalid data!");

            var tutor = await _context.Tutors.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (tutor == null) return NotFound("Tutor not found!");

            tutor.Department = registerDto.Department;
            tutor.ExperienceYears = registerDto.ExperienceYears ?? 0;
            tutor.Rating = registerDto.Rating ?? 0;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Tutor updated successfully!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-student/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound("Student not found!");

            if (student.User != null)
            {
                _context.Users.Remove(student.User);
            }
            var checkClassStudent = await _context.ClassStudents.AnyAsync(c => c.StudentId == id);
            if(checkClassStudent)
            {
                return BadRequest("Cannot delete this student because they are currently enrolled to one or more classes.");
            }
            var getUserId = student.UserId;
            var userNotification = await _context.Notifications.Where(n => n.UserId == getUserId || n.SenderId == getUserId).ToListAsync();
            var userBlog = await _context.Blogs.Where(b => b.UserId == getUserId).ToListAsync();
            var userBlogIds = userBlog.Select(b => b?.Id).ToList();
            var commentsToDelete = await _context.Comments.Where(c => userBlogIds.Contains(c.BlogId)).ToListAsync();
            var userComment = await _context.Comments.Where(c => c.UserId == getUserId).ToListAsync();
            var relatedMessages = await _context.Messages.Where(m => m.SenderId == getUserId || m.ReceiverId == getUserId).ToListAsync();
            _context.Notifications.RemoveRange(userNotification);
            _context.Comments.RemoveRange(userComment);
            _context.Comments.RemoveRange(commentsToDelete);
            _context.Blogs.RemoveRange(userBlog);
            _context.Messages.RemoveRange(relatedMessages);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Student deleted successfully!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-tutor/{id}")]
        public async Task<IActionResult> DeleteTutor(int id)
        {
            var tutor = await _context.Tutors.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (tutor == null) return NotFound("Tutor not found!");
            var checkClassTutor = await _context.Classes.AnyAsync(c => c.TutorId == id);
            var getUserId = tutor.UserId;
            var userNotification = await _context.Notifications.Where(n => n.UserId == getUserId || n.SenderId == getUserId).ToListAsync();
            var userBlog = await _context.Blogs.Where(b => b.UserId == getUserId).ToListAsync();
            var userBlogIds = userBlog.Select(b => b?.Id).ToList();
            var commentsToDelete = await _context.Comments.Where(c => userBlogIds.Contains(c.BlogId)).ToListAsync();
            var userComment = await _context.Comments.Where(c => c.UserId == getUserId).ToListAsync();
            var relatedMessages = await _context.Messages.Where(m => m.SenderId == getUserId || m.ReceiverId == getUserId).ToListAsync();
            _context.Notifications.RemoveRange(userNotification);
            _context.Comments.RemoveRange(userComment);
            _context.Comments.RemoveRange(commentsToDelete);
            _context.Blogs.RemoveRange(userBlog);
            _context.Messages.RemoveRange(relatedMessages);
            if (checkClassTutor)
            {
                return BadRequest("Cannot delete this tutor because they are currently assigned to one or more classes.");
            }
            if (tutor.User != null)
            {
                _context.Users.Remove(tutor.User);
            }

            _context.Tutors.Remove(tutor);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Tutor deleted successfully!" });
        }
    }
}
