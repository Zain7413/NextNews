using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextNews.Models.Database;
using NextNews.Services;

namespace NextNews.Controllers
{

    //// For Admin-specific actions
    //[Authorize(Policy = "Admin")]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        private readonly IRoleManagementService _roleManagementService;

        public RoleController(RoleManager<IdentityRole> roleManager , IRoleManagementService roleManagementService)
        {
            _roleManager = roleManager;
            _roleManagementService = roleManagementService;
           
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }


        public IActionResult ListOfRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }



        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }



        // This is for creating dynamic role wihout seeding

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _roleManagementService.CreateRoleAsync(roleName);

            if (result)
            {

                return RedirectToAction("CreationSuccessfully", "Role");
            }
            else
            {

                return View("CreationFailed", "Role");
            }
        }


        [HttpGet]
        public IActionResult CreationSuccessfully()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Failed()
        {
            return View();
        }








    }
}
