using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Exceptions;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Controllers;

namespace ForumProject.Controllers.MVC
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IViewModelMapper _modelMapper;

        public UserController(IUserService userService,IViewModelMapper modelMapper)
        {
            _userService = userService;
            _modelMapper = modelMapper;
        }

        [RequireAuthorization]
        public IActionResult Profile()
        {
            var profile = _modelMapper.ToUserProfileViewModel(HttpContext.Items["UserProfile"] as UserProfile);

            return View(profile);
        }

        [RequireAuthorization]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfile profile)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", profile);
            }

            await _userService.UpdateUserProfileAsync(profile);
            return RedirectToAction("Profile");
        }

        //[RequireAuthorization(requireAdmin: true)]
        //public async Task<IActionResult> ManageUsers()
        //{
        //    var users = await _userService.GetAllUsersAsync();
        //    return View(users);
        //}

        //[RequireAuthorization(requireAdmin: true)]
        //public async Task<IActionResult> BanUser(int userId)
        //{
        //    try
        //    {
        //        await _userService.BanUserAsync(userId);
        //        return RedirectToAction("ManageUsers");
        //    }
        //    catch (EntityNotFoundException ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //}

        //[RequireAuthorization(requireAdmin: true)]
        //public async Task<IActionResult> PromoteUser(int userId)
        //{
        //    try
        //    {
        //        await _userService.PromoteUserToAdminAsync(userId);
        //        return RedirectToAction("ManageUsers");
        //    }
        //    catch (EntityNotFoundException ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //}
    }
}
