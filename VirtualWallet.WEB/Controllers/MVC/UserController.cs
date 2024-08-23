using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Controllers;
using VirtualWallet.WEB.Models.ViewModels;
using VirtualWallet.DATA.Models.Enums;

namespace ForumProject.Controllers.MVC
{
    public class UserController : BaseController
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
            var profile = _modelMapper.ToUserViewModel(CurrentUser);
            return View(profile);
        }

        [RequireAuthorization]
        public IActionResult EditProfile()
        {
            var profile = _modelMapper.ToUserProfileViewModel(CurrentUser.UserProfile);
            profile.UserId = CurrentUser.Id;

            return View(profile);
        }

        [RequireAuthorization]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel user, IFormFile profilePicture)
        {
            if (!ModelState.IsValid)
            {
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
        public async Task<IActionResult> BlockUser(int userId, string reason)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null || !user.IsSuccess)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ManageUsers");
            }

            user.Value.Role = UserRole.Blocked;

            var blockRecord = new BlockedRecord
            {
                UserId = userId,
                Reason = reason,
                BlockedDate = DateTime.UtcNow
            };

            user.Value.BlockedRecord = blockRecord;

            await _userService.UpdateUserAsync(user.Value);

            return RedirectToAction("ManageUsers");
        }


        [RequireAuthorization]
        public async Task<IActionResult> UnblockUser(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null || !user.IsSuccess)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ManageUsers");
            }

            user.Value.Role = UserRole.RegisteredUser;

            if (user.Value.BlockedRecord != null)
            {
                user.Value.BlockedRecord=null;
            }

            await _userService.UpdateUserAsync(user.Value);

            return RedirectToAction("ManageUsers");
        }


        [HttpGet]
        public async Task<IActionResult> UnverifiedUsers()
        {
            var users = await _userService.GetUsers();
            var unverifiedUsers = users.Value
                .Where(u => u.VerificationStatus == UserVerificationStatus.PendingVerification)
                .Select(u => new UserVerificationViewModel
                {
                    UserId = u.Id,
                    Username = u.Username,
                    PhotoIdUrl = u.PhotoIdUrl,
                    LicenseIdUrl = u.FaceIdUrl,
                    VerificationStatus = u.VerificationStatus
                }).ToList();

            return View(unverifiedUsers);
        }



        [HttpPost]
        public async Task<IActionResult> VerifyUser(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.Value.VerificationStatus = UserVerificationStatus.Verified;
                await _userService.UpdateUserAsync(user.Value);
            }

            return RedirectToAction("UnverifiedUsers");
        }

        [HttpPost]
        public async Task<IActionResult> DenyUserVerification(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.Value.VerificationStatus = UserVerificationStatus.NotVerified;
                await _userService.UpdateUserAsync(user.Value);
            }

            return RedirectToAction("UnverifiedUsers");
        }
        [RequireAuthorization]
        [HttpPost]
        public async Task<IActionResult> AddFriend(int contactId)
        {
            var userId = CurrentUser.Id;

            var result = await _userService.AddFriendAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile");
            }

            return RedirectToAction("Profile");
        }

        [RequireAuthorization]
        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int contactId)
        {
            var userId = CurrentUser.Id;

            var result = await _userService.RemoveFriendAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile");
            }

            return RedirectToAction("Profile");
        }

        [RequireAuthorization]
        [HttpGet]
        public async Task<IActionResult> GetFriends()
        {
            var userId = CurrentUser.Id;

            var friends = await _userService.GetFriendsAsync(userId);

            var friendViewModels = friends.Select(f => new UserViewModel
            {
                Id = f.Id,
                Username = f.Username,
                Email = f.Email
            }).ToList();

            return View(friendViewModels);
        }
    }
}
