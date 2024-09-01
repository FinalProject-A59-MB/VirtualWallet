using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.WEB.Controllers.MVC;
using VirtualWallet.WEB.Models.ViewModels.UserViewModels;
using VirtualWallet.WEB.Models.ViewModels.CardViewModels;
using VirtualWallet.WEB.Models.ViewModels.WalletViewModels;
using VirtualWallet.WEB.Models.ViewModels.AdminViewModels;

namespace ForumProject.Controllers.MVC
{
    [RequireAuthorization(minRequiredRoleLevel: 1)]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IViewModelMapper _modelMapper;
        private readonly IWalletService _walletService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IAuthService _authService;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly ICardTransactionService _cardTransactionService;

        public UserController(
            IUserService userService,
            IViewModelMapper modelMapper,
            IWalletService walletService,
            ICloudinaryService cloudinaryService,
            IAuthService authService,
            IWalletTransactionService walletTransactionService,
            ICardTransactionService cardTransactionService)
        {
            _userService = userService;
            _modelMapper = modelMapper;
            _walletService = walletService;
            _cloudinaryService = cloudinaryService;
            _authService = authService;
            _walletTransactionService = walletTransactionService;
            _cardTransactionService = cardTransactionService;
        }

        public async Task<IActionResult> Profile(int? id)
        {
            UserViewModel profileViewModel;
            if (id.HasValue & id != 0)
            {

                Result<User> result = await _userService.GetUserByIdAsync(id.Value);

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
            
            var totalBalanceResult = await _userService.GetTotalBalanceInMainWalletCurrencyAsync(profileViewModel.Id);

            profileViewModel.TotalBalance = totalBalanceResult.Value.TotalAmount;


            return View(profileViewModel);
        }

        [RequireAuthorization(minRequiredRoleLevel: 2)]

        public IActionResult EditProfile()
        {
            UserProfileViewModel profile = _modelMapper.ToUserProfileViewModel(CurrentUser.UserProfile);
            profile.UserId = CurrentUser.Id;

            return View(profile);
        }
        [RequireAuthorization(minRequiredRoleLevel: 2)]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel userProfilemodel)
        {
            if (!ModelState.IsValid)
            {
                return View("EditProfile", userProfilemodel);
            }
            string username = userProfilemodel.UserName;
            if (userProfilemodel.file != null)
            {
                string imageUrl = _cloudinaryService.UploadProfilePicture(userProfilemodel.file);
                userProfilemodel.PhotoUrl = imageUrl;
            }
            UserProfile userProfil = _modelMapper.ToUserProfile(userProfilemodel);
            var userResult = await _userService.GetUserByIdAsync(userProfilemodel.UserId);
            User user = userResult.Value;
            user.UserProfile = userProfil;
            user.Username = username;
            await _userService.UpdateUserAsync(user);
            string token = _authService.GenerateToken(user);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });
            return RedirectToAction("Profile");
        }

        public IActionResult ChangePassword(int userId)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                UserId = userId,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

                string errorMessage = string.Join("\n", errors);

                TempData["ErrorMessage"] = errorMessage;

                return RedirectToAction("Profile", "User");
            }


            int userId = CurrentUser.Id;
            var result = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile", "User");
            }

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction("Profile", "User");
        }


        public IActionResult ChangeEmail(int userId)
        {

            ChangeEmailViewModel model = new ChangeEmailViewModel
            {
                UserId = userId,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

                string errorMessage = string.Join("\n", errors);

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("Profile", "User");
            }

            int userId = CurrentUser.Id;
            var result = await _userService.ChangeEmailAsync(userId, model.NewEmail, model.CurrentPassword);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile", "User");
            }

            TempData["SuccessMessage"] = "Email changed successfully.";
            return RedirectToAction("Profile", "User");
        }

        public IActionResult UploadVerification()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadVerificationDocuments(VerificationViewModel model)
        {
            if (model.PhotoId == null || model.LicenseId == null)
            {
                return View(model);
            }

            User user = HttpContext.Items["CurrentUser"] as User;

            string photoIdUrl = _cloudinaryService.UploadProfilePicture(model.PhotoId);
            user.PhotoIdUrl = photoIdUrl;

            string licenseIdUrl = _cloudinaryService.UploadProfilePicture(model.LicenseId);
            user.FaceIdUrl = licenseIdUrl;

            user.VerificationStatus = UserVerificationStatus.PendingVerification;
            var result = await _userService.UpdateUserAsync(user);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile", "User");
            }
            

            TempData["SuccessMessage"] = "Documents succesfully uploaded";
            return RedirectToAction("Profile", "User");
        }
        [RequireAuthorization(minRequiredRoleLevel: 5)]

        [HttpGet]
        public async Task<IActionResult> BlockUser(int userId)
        {
            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
            {
                TempData["ErrorMessage"] = userResult.Error;
                return RedirectToAction("ManageUsers");
            }

            BlockUserViewModel model = new BlockUserViewModel
            {
                UserId = userId,
                Username = userResult.Value.Username
            };

            return View("BlockUser", model);
        }

        [RequireAuthorization(minRequiredRoleLevel: 5)]

        [HttpPost]
        public async Task<IActionResult> BlockUser(BlockUserViewModel model)
        {
            Result<User> user = await _userService.GetUserByIdAsync(model.UserId);
            if (!user.IsSuccess)
            {
                TempData["ErrorMessage"] = user.Error;
                RedirectToAction("Profile", new { id = model.UserId });
            }

            user.Value.Role = UserRole.Blocked;

            BlockedRecord blockRecord = new BlockedRecord
            {
                UserId = model.UserId,
                Reason = model.Reason,
                BlockedDate = DateTime.UtcNow
            };

            user.Value.BlockedRecord = blockRecord;

            await _userService.UpdateUserAsync(user.Value);

            TempData["SuccessMessage"] = $"User {user.Value.Username} has been blocked successfully.";
            return RedirectToAction("Profile", new { id = model.UserId });
        }



        [RequireAuthorization(minRequiredRoleLevel: 5)]
        [HttpGet]
        public async Task<IActionResult> UnblockUser(int userId)
        {
            Result<User> user = await _userService.GetUserByIdAsync(userId);
            if (!user.IsSuccess)
            {
                TempData["ErrorMessage"] = user.Error;
                return RedirectToAction("ManageUsers");
            }

            BlockUserViewModel model = new BlockUserViewModel
            {
                UserId = userId,
                Username = user.Value.Username
            };

            return View("UnblockUser", model);
        }

        [RequireAuthorization(minRequiredRoleLevel: 5)]
        [HttpPost]
        public async Task<IActionResult> UnblockUser(BlockUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UnblockUser", model);
            }

            Result<User> user = await _userService.GetUserByIdAsync(model.UserId);
            if (!user.IsSuccess)
            {
                TempData["ErrorMessage"] = user.Error;
                return RedirectToAction("Profile", new { id = model.UserId });
            }

            user.Value.Role = UserRole.RegisteredUser;

            if (user.Value.BlockedRecord != null)
            {
                user.Value.BlockedRecord.Reason += $" --- Unban Reason: {model.Reason}";
            }

            user.Value.BlockedRecord = null;

            await _userService.UpdateUserAsync(user.Value);

            TempData["SuccessMessage"] = "User has been unblocked successfully.";
            return RedirectToAction("Profile",new {id = model.UserId});
        }



        [RequireAuthorization(minRequiredRoleLevel: 5)]
        [HttpGet]
        public async Task<IActionResult> UnverifiedUsers()
        {
            Result<IEnumerable<User>> users = await _userService.GetUsers();
            List<UserVerificationViewModel> unverifiedUsers = users.Value
                .Where(u => u.VerificationStatus == UserVerificationStatus.PendingVerification)
                .Select(_modelMapper.ToUserVerificationViewModel).ToList();


            return View(unverifiedUsers);
        }


        [RequireAuthorization(minRequiredRoleLevel: 5)]
        [HttpPost]
        public async Task<IActionResult> VerifyUser(int userId)
        {
            Result<User> user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.Value.VerificationStatus = UserVerificationStatus.Verified;
                await _userService.UpdateUserAsync(user.Value);
            }

            return RedirectToAction("UnverifiedUsers");
        }

        [RequireAuthorization(minRequiredRoleLevel: 5)]
        [HttpPost]
        public async Task<IActionResult> DenyUserVerification(int userId)
        {
            Result<User> user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.Value.VerificationStatus = UserVerificationStatus.NotVerified;
                await _userService.UpdateUserAsync(user.Value);
            }

            return RedirectToAction("UnverifiedUsers");
        }

        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int contactId)
        {
            int userId = CurrentUser.Id;
            var result = await _userService.SendFriendRequestAsync( userId, contactId);

            if (!result.IsSuccess)
            {
                TempData["InfoMessage"] = result.Error;
                return RedirectToAction("Profile", new { id = contactId });
            }

            TempData["SuccessMessage"] = "Friend request sent!";
            return RedirectToAction("Profile", new { id = contactId });
        }

        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(int contactId)
        {
            int userId = CurrentUser.Id;
            var result = await _userService.AcceptFriendRequestAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile");
            }

            return RedirectToAction("Profile");
        }

        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpPost]
        public async Task<IActionResult> DenyFriendRequest(int contactId)
        {
            int userId = CurrentUser.Id;
            var result = await _userService.DenyFriendRequestAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile");
            }

            return RedirectToAction("Profile");
        }

        [RequireAuthorization(minRequiredRoleLevel: 3)]
        public async Task<IActionResult> PendingFriendRequests()
        {
            int userId = CurrentUser.Id;
            Result<IEnumerable<UserContact>> result = await _userService.GetPendingFriendRequestsAsync(userId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Profile");
            }

            return View(result.Value);
        }


        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpGet]
        public IActionResult TransactionLog(TransactionLogViewModel model)
        {
            return View("TransactionLog", model);
        }

        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return View(Enumerable.Empty<UserViewModel>());
            }

            Result<IEnumerable<User>> result = await _userService.SearchUsersAsync(searchTerm);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return View(Enumerable.Empty<UserViewModel>());
            }

            IEnumerable<UserViewModel> userViewModels = result.Value.Select(_modelMapper.ToUserViewModel);

            return View(userViewModels);
        }

        [RequireAuthorization(minRequiredRoleLevel: 2)]
        public async Task<IActionResult> Cards()
        {
            if (!CurrentUser.Cards.Any())
            {
                TempData["InfoMessage"] = "Currently you do not have any cards. You will first need to add a card.";
                return RedirectToAction("AddCard", "Card", new { userId = CurrentUser.Id });
            }
            UserViewModel viewModel = _modelMapper.ToUserViewModel(CurrentUser);

            return View("UserCards", viewModel);
        }

        [RequireAuthorization(minRequiredRoleLevel: 2)]
        public async Task<IActionResult> Wallets()
        {
            User user = CurrentUser;

            UserViewModel userViewModel = _modelMapper.ToUserViewModel(user);

            return View("UserWallets", userViewModel);
        }

        [RequireAuthorization(minRequiredRoleLevel: 2)]
        [HttpPost]
        public async Task<IActionResult> UpdateFriendDescription(int contactId, string description)
        {
            int userId = CurrentUser.Id;
            var result = await _userService.UpdateContact(userId, contactId, description);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("Profile", new { id = userId });
        }

        [RequireAuthorization(minRequiredRoleLevel: 1)]
        [HttpGet]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            Result<User> user = await _userService.GetUserByIdAsync(id);
            if (!user.IsSuccess)
            {
                TempData["ErrorMessage"] = user.Error;
                return RedirectToAction("Profile", new { id = id });
            }
            decimal totalAmmout = user.Value.Wallets.Select(w => w.Balance).Sum();
            if (totalAmmout > 0)
            {
                TempData["ErrorMessage"] = "There are still funds in your wallets.\nPlease Withdraw all funds from your wallets before you can proceed.";
                return RedirectToAction("Profile", new { id = id });
            }
            DeleteAccountViewModel model = new DeleteAccountViewModel
            {
                Id = user.Value.Id,
                Username = user.Value.Username,
                Email = user.Value.Email
            };

            return View(model);
        }

        [RequireAuthorization(minRequiredRoleLevel: 1)]
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


        [RequireAuthorization(minRequiredRoleLevel: 5)]
        [HttpGet]
        public async Task<IActionResult> AdminPanel(UserQueryParameters userParameters, TransactionQueryParameters walletTransactionParameters, CardTransactionQueryParameters cardTransactionParameters)
        {
            Result<IEnumerable<User>> usersResult = await _userService.FilterUsersAsync(userParameters);
            IEnumerable<UserViewModel> userViewModels = usersResult.IsSuccess
                ? usersResult.Value.Select(_modelMapper.ToUserViewModel).ToList()
                : Enumerable.Empty<UserViewModel>();

            Result<int> totalUserCountResult = await _userService.GetTotalUserCountAsync(userParameters);
            int totalUserCount = totalUserCountResult.IsSuccess ? totalUserCountResult.Value : 0;

            Result<IEnumerable<WalletTransaction>> walletTransactionsResult = await _walletTransactionService.FilterWalletTransactionsAsync(walletTransactionParameters);
            IEnumerable<WalletTransactionViewModel> walletTransactionViewModels = walletTransactionsResult.IsSuccess
                ? walletTransactionsResult.Value.Select(_modelMapper.ToWalletTransactionViewModel).ToList()
                : Enumerable.Empty<WalletTransactionViewModel>();

            Result<int> totalWalletTransactionCountResult = await _walletTransactionService.GetTotalCountAsync(walletTransactionParameters);
            int totalWalletTransactionCount = totalWalletTransactionCountResult.IsSuccess ? totalWalletTransactionCountResult.Value : 0;

            Result<IEnumerable<CardTransaction>> cardTransactionsResult = await _cardTransactionService.FilterCardTransactionsAsync(cardTransactionParameters);
            IEnumerable<CardTransactionViewModel> cardTransactionViewModels = cardTransactionsResult.IsSuccess
                ? cardTransactionsResult.Value.Select(_modelMapper.ToCardTransactionViewModel).ToList()
                : Enumerable.Empty<CardTransactionViewModel>();

            Result<int> totalCardTransactionCountResult = await _cardTransactionService.GetCardTransactionTotalCountAsync(cardTransactionParameters);
            int totalCardTransactionCount = totalCardTransactionCountResult.IsSuccess ? totalCardTransactionCountResult.Value : 0;

            var viewModel = new AdminPanelViewModel
            {
                Users = userViewModels,
                UsersTotalPages = (int)Math.Ceiling(totalUserCount / (double)userParameters.PageSize),
                UsersCurrentPage = userParameters.PageNumber,

                WalletTransactions = walletTransactionViewModels,
                WalletTransactionsTotalPages = (int)Math.Ceiling(totalWalletTransactionCount / (double)walletTransactionParameters.PageSize),
                WalletTransactionsCurrentPage = walletTransactionParameters.PageNumber,

                CardTransactions = cardTransactionViewModels,
                CardTransactionsTotalPages = (int)Math.Ceiling(totalCardTransactionCount / (double)cardTransactionParameters.PageSize),
                CardTransactionsCurrentPage = cardTransactionParameters.PageNumber,
            };

            return View(viewModel);
        }


    }
}
