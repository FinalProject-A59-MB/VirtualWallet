using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Controllers;
using VirtualWallet.WEB.Models.ViewModels;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.BUSINESS.Results;

namespace ForumProject.Controllers.MVC
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IViewModelMapper _modelMapper;
        private readonly IWalletService _walletService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IAuthService _authService;

        public UserController(
            IUserService userService,
            IViewModelMapper modelMapper,
            IWalletService walletService,
            ICloudinaryService cloudinaryService,
            IAuthService authService)
        {
            _userService = userService;
            _modelMapper = modelMapper;
            _walletService = walletService;
            _cloudinaryService = cloudinaryService;
            _authService = authService;
        }

        [RequireAuthorization]
        public async Task<IActionResult> Profile(int? id)
        {
            UserViewModel profileViewModel;

            if (id.HasValue & id != 0)
            {

                var result = await _userService.GetUserByIdAsync(id.Value);

                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Error;
                    return RedirectToAction("Index", "Home");
                }


                profileViewModel = _modelMapper.ToUserViewModel(result.Value);
            }
            else
            {

                profileViewModel = _modelMapper.ToUserViewModel(CurrentUser);
            }

            profileViewModel.TotalBalance = profileViewModel.Wallets.Select(x => x.Balance).Sum();


            return View(profileViewModel);
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
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel userProfilemodel)
        {
            if (!ModelState.IsValid)
            {
                return View("EditProfile", userProfilemodel);
            }
            var username = userProfilemodel.UserName;
            if (userProfilemodel.file != null)
            {
                var imageUrl = _cloudinaryService.UploadProfilePicture(userProfilemodel.file);
                userProfilemodel.PhotoUrl = imageUrl;
            }
            var userProfil = _modelMapper.ToUserProfile(userProfilemodel);
            var userResult = await _userService.GetUserByIdAsync(userProfilemodel.UserId);
            var user = userResult.Value;
            user.UserProfile = userProfil;
            user.Username = username;
            await _userService.UpdateUserAsync(user);
            var token = _authService.GenerateToken(user);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });
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
            if (!user.IsSuccess)
            {
                TempData["ErrorMessage"] = user.Error;
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
                TempData["ErrorMessage"] = user.Error;
                return RedirectToAction("ManageUsers");
            }

            user.Value.Role = UserRole.RegisteredUser;

            if (user.Value.BlockedRecord != null)
            {
                user.Value.BlockedRecord = null;
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
        public async Task<IActionResult> SendFriendRequest(int contactId)
        {
            var userId = CurrentUser.Id;
            var result = await _userService.SendFriendRequestAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile", new { id = contactId });
            }

            TempData["SuccessMessage"] = "Friend request sent!";
            return RedirectToAction("Profile", new { id = contactId });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(int contactId)
        {
            var userId = CurrentUser.Id;
            var result = await _userService.AcceptFriendRequestAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile");
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> DenyFriendRequest(int contactId)
        {
            var userId = CurrentUser.Id;
            var result = await _userService.DenyFriendRequestAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile");
            }

            return RedirectToAction("Profile");
        }


        public async Task<IActionResult> PendingFriendRequests()
        {
            var userId = CurrentUser.Id;
            var result = await _userService.GetPendingFriendRequestsAsync(userId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile");
            }

            return View(result.Value);
        }

        [HttpGet]
        public IActionResult TransactionLog(TransactionLogViewModel model)
        {
            return View("TransactionLog", model);
        }


        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return View(Enumerable.Empty<UserViewModel>());
            }

            var result = await _userService.SearchUsersAsync(searchTerm);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return View(Enumerable.Empty<UserViewModel>());
            }

            var userViewModels = result.Value.Select(u => new UserViewModel
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
            });

            return View(userViewModels);
        }

        [RequireAuthorization]
        public async Task<IActionResult> Cards()
        {
            if (!CurrentUser.Cards.Any())
            {
                TempData["InfoMessage"] = "Currently you do not have any cards. You will first need to add a card.";
                return RedirectToAction("AddCard", "Card", new { userId = CurrentUser.Id });
            }
            var viewModel = _modelMapper.ToUserViewModel(CurrentUser);

            return View("UserCardsPartial", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFriendDescription(int contactId, string description)
        {
            var userId = CurrentUser.Id;
            var result = await _userService.UpdateContact(userId, contactId, description);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("Profile", new { id = userId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (!user.IsSuccess)
            {
                TempData["ErrorMessage"] = user.Error;
                return RedirectToAction("Profile", new { id = id });
            }
            var totalAmmout = user.Value.Wallets.Select(w => w.Balance).Sum();
            if (totalAmmout > 0)
            {
                TempData["ErrorMessage"] = "There are still funds in your wallets.\nPlease Withdraw all funds from your wallets before you can proceed.";
                return RedirectToAction("Profile", new { id = id });
            }
            var model = new DeleteAccountViewModel
            {
                Id = user.Value.Id,
                Username = user.Value.Username,
                Email = user.Value.Email
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("DeleteAccount", new { id });
            }

            TempData["SuccessMessage"] = "Account deleted successfully.";
            return RedirectToAction("Index", "Home");
        }

    }
}
