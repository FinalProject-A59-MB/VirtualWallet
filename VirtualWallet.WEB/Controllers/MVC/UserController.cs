using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Exceptions;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Controllers;
using VirtualWallet.WEB.Models.ViewModels;
using VirtualWallet.DATA.Models.Enums;
using System.ComponentModel;
using System.Linq;

namespace ForumProject.Controllers.MVC
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IViewModelMapper _modelMapper;
        private readonly IWalletService _walletService;
        private readonly ICloudinaryService _cloudinaryService;

        public UserController(
            IUserService userService,
            IViewModelMapper modelMapper, 
            IWalletService walletService,
            ICloudinaryService cloudinaryService)
        {
            _userService = userService;
            _modelMapper = modelMapper;
            _walletService = walletService;
            _cloudinaryService = cloudinaryService;
        }

        [RequireAuthorization]
        public async Task<IActionResult> Profile()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            var profile = _modelMapper.ToUserViewModel(user);

            var users = await _userService.GetUsers();
            var filteredUsers = users
                .Where(u => u.VerificationStatus == UserVerificationStatus.PendingVerification)
                .ToList();
            ViewData["UnverifiedUsers"] = filteredUsers;
            return View(profile);
        }

        [RequireAuthorization]
        public IActionResult EditProfile()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            var profile = _modelMapper.ToUserProfileViewModel(user.UserProfile);
            profile.UserId = user.Id;

            return View(profile);
        }

        [RequireAuthorization]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel user, IFormFile profilePicture)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.ToList();
                return View("EditProfile", user);
            }
            if (profilePicture != null)
            {
                var imageUrl = _cloudinaryService.UploadProfilePicture(profilePicture);
                user.PhotoUrl = imageUrl;
            }

            var userProfile = _modelMapper.ToUserProfile(user);
            await _userService.UpdateUserProfileAsync(userProfile);

            return RedirectToAction("Profile");
        }

        [RequireAuthorization]
        public IActionResult UploadVerification()
        {
            return View();
        }

        [RequireAuthorization]
        [HttpPost]
        public async Task<IActionResult> UploadVerificationDocuments(VerificationViewModel model)
        {
            if (model.PhotoId == null || model.LicenseId == null)
            {
                //ModelState.AddModelError("", "Both Photo ID and License ID are required.");
                return View(model);
            }

            var user = HttpContext.Items["CurrentUser"] as User;

            var photoIdUrl = _cloudinaryService.UploadProfilePicture(model.PhotoId);
            user.PhotoIdUrl = photoIdUrl;

            var licenseIdUrl = _cloudinaryService.UploadProfilePicture(model.LicenseId);
            user.FaceIdUrl = licenseIdUrl;

            user.VerificationStatus = UserVerificationStatus.PendingVerification;
            await _userService.UpdateUserAsync(user);

            return RedirectToAction("Profile", "User");
        }

        [RequireAuthorization]
        public async Task<IActionResult> BlockUser(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            user.Role = UserRole.Blocked;
            await _userService.UpdateUserAsync(user);

            return RedirectToAction("ManageUsers");
        }

        [RequireAuthorization]
        public async Task<IActionResult> UnblockUser(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            user.Role = UserRole.RegisteredUser;
            await _userService.UpdateUserAsync(user);

            return RedirectToAction("ManageUsers");
        }

        [HttpGet]
        public async Task<IActionResult> UnverifiedUsers()
        {
            var users = await _userService.GetUsers();

            var filteredUsers = users
                .Where(u => u.VerificationStatus == UserVerificationStatus.PendingVerification)
                .ToList();

            var viewModel = new UnverifiedUsersViewModel
            {
                Users = filteredUsers.Select(u => new UserVerificationViewModel
                {
                    UserId = u.Id,
                    Username = u.Username,
                    PhotoIdUrl = u.PhotoIdUrl,
                    LicenseIdUrl = u.FaceIdUrl,
                    VerificationStatus = u.VerificationStatus
                }).ToList()
            };

            return View(viewModel);
        }



        [HttpGet]
        public async Task<IActionResult> VerifyUser(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null || user.VerificationStatus != UserVerificationStatus.PendingVerification)
            {
                return NotFound();
            }

            var model = new UserVerificationViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                PhotoIdUrl = user.PhotoIdUrl,
                LicenseIdUrl = user.FaceIdUrl,
                VerificationStatus = user.VerificationStatus
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyUser(UserVerificationViewModel model, bool isVerified)
        {
            var user = await _userService.GetUserByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.VerificationStatus = isVerified ? UserVerificationStatus.Verified : UserVerificationStatus.PendingVerification;
            await _userService.UpdateUserAsync(user);

            return RedirectToAction("UnverifiedUsers");
        }
    }
}
