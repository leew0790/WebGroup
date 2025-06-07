﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectComp1640.Data;
using ProjectComp1640.Dtos.Account;
using ProjectComp1640.Interfaces;
using ProjectComp1640.Model;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectComp1640.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly ApplicationDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            ApplicationDBContext context,
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            SignInManager<AppUser> signInManager,
            EmailService emailService,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = loginDto.Username?.ToLower();
            if (string.IsNullOrEmpty(username))
                return BadRequest(new{ message = "Username is required." });

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null) return Unauthorized(new { message = "Invalid username!" });

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized(new { message = "Invalid username or password!" });

            return Ok(new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user)
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 🔥 Kiểm tra định dạng email hợp lệ
            if (!Regex.IsMatch(registerDto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("Invalid email format.");
            // Kiểm tra username & email đã tồn tại chưa
            if (await _userManager.FindByNameAsync(registerDto.Username) != null)
                return BadRequest("Username is already taken.");
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                return BadRequest("Email is already in use.");
            if (await _context.Students.AnyAsync(s => s.StudentCode == registerDto.StudentCode))
                return BadRequest("Student code already exists.");

            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Kiểm tra và tạo vai trò nếu chưa có
            if (!await _roleManager.RoleExistsAsync("Student"))
                await _roleManager.CreateAsync(new IdentityRole("Student"));

            await _userManager.AddToRoleAsync(user, "Student");

            var student = new Student
            {
                UserId = user.Id,
                StudentCode = registerDto.StudentCode,
                Course = registerDto.Course,
                Status = registerDto.Status
            };

            _context.Students.Add(student);
            try
            {
                await _context.SaveChangesAsync();

                await SendEmailRegister(registerDto.Email);

                return Ok(new { Message = "Student registered successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while saving data: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-tutor")]
        public async Task<IActionResult> RegisterTutor([FromBody] RegisterTutorDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // 🔥 Kiểm tra định dạng email hợp lệ
            if (!Regex.IsMatch(registerDto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("Invalid email format.");
            // Kiểm tra username & email đã tồn tại chưa
            if (await _userManager.FindByNameAsync(registerDto.Username) != null)
                return BadRequest("Username is already taken.");
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                return BadRequest("Email is already in use.");

            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Kiểm tra và tạo vai trò nếu chưa có
            if (!await _roleManager.RoleExistsAsync("Tutor"))
                await _roleManager.CreateAsync(new IdentityRole("Tutor"));

            await _userManager.AddToRoleAsync(user, "Tutor");

            var tutor = new Tutor
            {
                UserId = user.Id,
                Department = registerDto.Department,
                ExperienceYears = registerDto.ExperienceYears ?? 0,
                Rating = registerDto.Rating ?? 0
            };

            _context.Tutors.Add(tutor);
            try
            {
                await _context.SaveChangesAsync();

                await SendEmailRegister(registerDto.Email);

                return Ok(new { Message = "Tutor registered successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while saving data: " + ex.Message);
            }
        }

        private async Task SendEmailRegister(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return;
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return;
            }

            // Tạo token đặt lại mật khẩu
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            // Đường dẫn frontend kèm email + token
            string resetLink = $"https://victorious-smoke-0d0ea8a00.6.azurestaticapps.net/reset-password?email={email}&token={encodedToken}";

            string subject = "Đăng ký thành công";
            string body = "<p>Bạn đã được đăng ký tài khoản thành công.</p>" +
                          "<p>Vui lòng nhấp vào liên kết dưới đây để đặt mật khẩu của bạn:</p>" +
                          $"<a href='{resetLink}'>Đặt lại mật khẩu</a>";

            await _emailService.SendEmailAsync(email, subject, body);
        }


        [Authorize] // Yêu cầu người dùng đã đăng nhập mới có thể gọi API này
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync(); // Đăng xuất người dùng hiện tại
            return Ok(new { Message = "Logged out successfully!" });
        }

    }
}
