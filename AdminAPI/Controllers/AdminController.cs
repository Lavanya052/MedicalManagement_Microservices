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

        [HttpGet("getalladmin")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAdmins();
            return Ok(admins);
        }

        [HttpGet("getadminbyid/{id}")]
        public async Task<IActionResult> GetAdminById([FromRoute] string id)
        {
            var admin = await _adminService.GetAdminById(id);
            if (admin == null) return NotFound();

            return Ok(admin);
        }

        [HttpPost("createadmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] Admin admin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAdmin = await _adminService.CreateAdmin(admin);
            return Ok(createdAdmin);
        }


        [HttpPut("updateadmin/{id}")]
        public async Task<IActionResult> UpdateAdmin([FromRoute] string id, Admin updatedAdmin)
        {
            var admin = await _adminService.UpdateAdmin(id, updatedAdmin);
            if (admin == null) return NotFound();

            return Ok(admin);
        }

        [HttpDelete("deleteadmin/{id}")]
        public async Task<IActionResult> DeleteAdmin([FromRoute] string id)
        {
            var isDeleted = await _adminService.DeleteAdmin(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }
    }
}