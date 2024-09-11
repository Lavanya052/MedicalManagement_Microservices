using AdminAPI.Models;
using AdminAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAdmins();
            return Ok(admins);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminById(string id)
        {
            var admin = await _adminService.GetAdminById(id);
            if (admin == null) return NotFound();

            return Ok(admin);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(Admin admin)
        {
            var createdAdmin = await _adminService.CreateAdmin(admin);
            return CreatedAtAction(nameof(GetAdminById), new { id = createdAdmin.Id }, createdAdmin);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(string id, Admin updatedAdmin)
        {
            var admin = await _adminService.UpdateAdmin(id, updatedAdmin);
            if (admin == null) return NotFound();

            return Ok(admin);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var isDeleted = await _adminService.DeleteAdmin(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }
    }
}
