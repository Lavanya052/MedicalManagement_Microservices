using AdminAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers
{
    [Route("api/admin/internal")]
    [ApiController]
    public class AdminInternalController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminInternalController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminByUsername(string id)
        {
            var admin = await _adminService.GetAdminByUsername(id);
            if (admin == null)
                return NotFound();

            return Ok(admin);
        }
    }

}
