using DataBase.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _UserManager;
        private SignInManager<IdentityUser> _SignInManager;
        private RoleManager<IdentityRole> _RoleManager;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _UserManager = userManager;
            _SignInManager = signInManager;
            _RoleManager = roleManager;
        }
        #region Register
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityUser user = new IdentityUser
                    {
                        Email = model.Email,
                        UserName = model.UserName,
                        PhoneNumber = model.Telephone,
                    };
                    var Result = await _UserManager.CreateAsync(user, model.Password);

                    if (Result.Succeeded)
                    {
                        await _SignInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View(model);
        }
        #endregion
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #region Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var Result = await _SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                    if (Result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid UserName or Password");
                }

                return View(model);
            }

            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        public IActionResult RolesList()
        {
            var RoleList = _RoleManager.Roles.ToList();

            return View(RoleList);
        }

        #region RoleCreate & Edit 
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityRole role = new IdentityRole
                    {
                        Name = model.Name
                    };
                    var Result = await _RoleManager.CreateAsync(role);
                    if (Result.Succeeded)
                    {
                        return RedirectToAction("");
                    }

                    foreach (var Error in Result.Errors)
                    {
                        ModelState.AddModelError("Error", Error.Description);
                    }
                }
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {

            var role = await _RoleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            var model = new EditRoleViewModel
            {

                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var User in _UserManager.Users)
            {
                if (await _UserManager.IsInRoleAsync(User, role.Name))
                {
                    model.Users.Add(User.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var role = await _RoleManager.FindByIdAsync(model.Id);

                    if (role == null)
                        return NotFound();

                    role.Name = model.RoleName;

                    // Update Role in Table AspNetRole 

                    var Result = await _RoleManager.UpdateAsync(role);

                    if (Result.Succeeded)
                    {
                        return RedirectToAction(nameof(RolesList));

                    }
                }
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string id)
        {

            var Role =await _RoleManager.FindByIdAsync(id);

            if (Role == null)
            
                return NotFound();


            var model = new List<UserRoleViewModel>();

            foreach (var Users in _UserManager.Users)
            {

                var UserRoleViewModel = new UserRoleViewModel
                {
                    UserId = Users.Id,
                    UserName = Users.UserName
                };

                if (await _UserManager.IsInRoleAsync(Users , Role.Name))
                {
                    UserRoleViewModel.IsSelected = true;
                }
                else
                {
                    UserRoleViewModel.IsSelected = false;
                }
                model.Add(UserRoleViewModel);
            }

            return View();
        }
    }
}
