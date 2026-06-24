// File: Controllers/NoticeController.cs
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public NoticeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notices = await _context.Notices
                .OrderByDescending(n => n.Date)
                .ToListAsync();
            return Ok(notices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var notice = await _context.Notices.FindAsync(id);
            return notice == null ? NotFound() : Ok(notice);
        }

        [HttpPost("create-with-file")]
        public async Task<IActionResult> CreateWithFile([FromForm] IFormFile? image,
                                                        [FromForm] IFormFile? file,
                                                        [FromForm] string title,
                                                        [FromForm] string description,
                                                        [FromForm] DateTime date)
        {
            string? imageUrl = null;
            string? fileUrl = null;

            string uploadPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "notices");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            if (image != null && image.Length > 0)
            {
                string imageName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                string path = Path.Combine(uploadPath, imageName);
                using (var stream = new FileStream(path, FileMode.Create))
                    await image.CopyToAsync(stream);
                imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/notices/{imageName}";
            }

            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string path = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                    await file.CopyToAsync(stream);
                fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/notices/{fileName}";
            }

            var notice = new Notice
            {
                Title = title,
                Description = description,
                Date = date,
                ImageUrl = imageUrl,
                FileUrl = fileUrl
            };

            _context.Notices.Add(notice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = notice.NoticeId }, notice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notice = await _context.Notices.FindAsync(id);
            if (notice == null) return NotFound();

            _context.Notices.Remove(notice);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
