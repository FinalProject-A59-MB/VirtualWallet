using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.WEB.Models.DTOs.UserDTOs;
using VirtualWallet.WEB.Models.DTOs.CardDTOs;
using VirtualWallet.WEB.Models.DTOs.WalletDTOs;

namespace VirtualWallet.WEB.Controllers.API
{
    /// <summary>
    /// Controller responsible for managing user-related actions such as profile management, account settings, and admin-level operations.
    /// </summary>
    [Route("api/User")]
    [ApiController]
    [RequireAuthorization(minRequiredRoleLevel: 1)]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IDtoMapper _dtoMapper;
        private readonly IWalletService _walletService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IAuthService _authService;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly ICardTransactionService _cardTransactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">Service for managing users.</param>
        /// <param name="dtoMapper">Service for mapping DTOs to models.</param>
        /// <param name="walletService">Service for managing wallets.</param>
        /// <param name="cloudinaryService">Service for handling cloud storage operations.</param>
        /// <param name="authService">Service for managing authentication.</param>
        /// <param name="walletTransactionService">Service for managing wallet transactions.</param>
        /// <param name="cardTransactionService">Service for managing card transactions.</param>
        public UserController(
            IUserService userService,
            IDtoMapper dtoMapper,
            IWalletService walletService,
            ICloudinaryService cloudinaryService,
            IAuthService authService,
            IWalletTransactionService walletTransactionService,
            ICardTransactionService cardTransactionService)
        {
            _userService = userService;
            _dtoMapper = dtoMapper;
            _walletService = walletService;
            _cloudinaryService = cloudinaryService;
            _authService = authService;
            _walletTransactionService = walletTransactionService;
            _cardTransactionService = cardTransactionService;
        }

        /// <summary>
        /// Retrieves the profile information of a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user's profile information if found; otherwise, an error message.</returns>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, result.Error);
            }

            var profileDto = _dtoMapper.ToUserProfileResponseDto(result.Value.UserProfile);

            var totalBalanceResult = await _userService.GetTotalBalanceInMainWalletCurrencyAsync(profileDto.Id);

            if (!totalBalanceResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, totalBalanceResult.Error);
            }

            profileDto.TotalBalance = totalBalanceResult.Value.TotalAmount;

            return StatusCode(StatusCodes.Status200OK, profileDto);
        }

        /// <summary>
        /// Updates the user's profile information and optionally updates their profile picture.
        /// </summary>
        /// <param name="userProfileDto">The user's profile data to update.</param>
        /// <param name="userPicture">The user's new profile picture.</param>
        /// <returns>A status indicating whether the profile update was successful or not.</returns>
        [HttpPost("updateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileRequestDto userProfileDto, IFormFile userPicture)
        {
            if (userPicture != null)
            {
                var imageUrl = _cloudinaryService.UploadProfilePicture(userPicture);
                userProfileDto.PhotoUrl = imageUrl;
            }

            var userProfile = _dtoMapper.ToUserProfile(userProfileDto);
            var userResult = await _userService.GetUserByIdAsync(userProfileDto.Id);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, userResult.Error);
            }

            var user = userResult.Value;
            user.UserProfile = userProfile;

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, updateResult.Error);
            }

            var token = _authService.GenerateToken(user);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });

            return StatusCode(StatusCodes.Status200OK, "Profile updated successfully.");
        }

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="model">The password change request data.</param>
        /// <returns>A status indicating whether the password change was successful or not.</returns>
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            var userId = CurrentUser.Id;
            var result = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, result.Error);
            }

            return StatusCode(StatusCodes.Status200OK, "Password changed successfully.");
        }

        /// <summary>
        /// Changes the user's email address.
        /// </summary>
        /// <param name="model">The email change request data.</param>
        /// <returns>A status indicating whether the email change was successful or not.</returns>
        [HttpPost("changeEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequestDto model)
        {
            var userId = CurrentUser.Id;
            var result = await _userService.ChangeEmailAsync(userId, model.NewEmail, model.CurrentPassword);

            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, result.Error);
            }
            return StatusCode(StatusCodes.Status200OK, "Email changed successfully.");
        }

        /// <summary>
        /// Uploads user verification documents such as Photo ID and License ID.
        /// </summary>
        /// <param name="model">The verification documents.</param>
        /// <returns>A status indicating whether the upload was successful or not.</returns>
        [HttpPost("uploadVerification")]
        public async Task<IActionResult> UploadVerificationDocuments([FromBody] VerificationRequestDto model)
        {
            if (model.PhotoId == null || model.LicenseId == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Photo ID and License ID are required.");
            }

            var user = CurrentUser;

            var photoIdUrl = _cloudinaryService.UploadProfilePicture(model.PhotoId);
            user.PhotoIdUrl = photoIdUrl;

            var licenseIdUrl = _cloudinaryService.UploadProfilePicture(model.LicenseId);
            user.FaceIdUrl = licenseIdUrl;

            user.VerificationStatus = UserVerificationStatus.PendingVerification;
            var result = await _userService.UpdateUserAsync(user);

            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }

            return StatusCode(StatusCodes.Status200OK, "Documents successfully uploaded.");
        }

        /// <summary>
        /// Blocks a user, changing their role to Blocked and recording the block reason.
        /// </summary>
        /// <param name="model">The block record data, including user ID and reason.</param>
        /// <returns>A status indicating whether the user was successfully blocked or not.</returns>
        [RequireAuthorization]
        [HttpPost("blockUser")]
        public async Task<IActionResult> BlockUser([FromBody] BlockedRecordRequestDto model)
        {
            var userResult = await _userService.GetUserByIdAsync(model.UserId);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, userResult.Error);
            }

            var user = userResult.Value;
            user.Role = UserRole.Blocked;

            var blockRecord = new BlockedRecord
            {
                UserId = model.UserId,
                Reason = model.Reason,
                BlockedDate = DateTime.UtcNow
            };

            user.BlockedRecord = blockRecord;

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, updateResult.Error);
            }

            return StatusCode(StatusCodes.Status200OK, $"User {user.Username} has been blocked successfully.");
        }

        /// <summary>
        /// Unblocks a user, restoring their role to RegisteredUser.
        /// </summary>
        /// <param name="model">The unblock record data, including user ID and reason for unblocking.</param>
        /// <returns>A status indicating whether the user was successfully unblocked or not.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 4)]
        [HttpPost("unblockUser")]
        public async Task<IActionResult> UnblockUser([FromBody] UnblockRecordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            var userResult = await _userService.GetUserByIdAsync(model.UserId);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, userResult.Error);
            }

            var user = userResult.Value;
            user.Role = UserRole.RegisteredUser;

            if (user.BlockedRecord != null)
            {
                user.BlockedRecord.Reason += $" --- Unban Reason: {model.Reason}";
                user.BlockedRecord = null;
            }

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, updateResult.Error);
            }

            return StatusCode(StatusCodes.Status200OK, "User has been unblocked successfully.");
        }

        /// <summary>
        /// Retrieves a list of unverified users pending verification.
        /// </summary>
        /// <returns>A list of unverified users if found; otherwise, an error message.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpGet("unverifiedUsers")]
        public async Task<IActionResult> GetUnverifiedUsers()
        {
            var usersResult = await _userService.GetUsers();

            if (!usersResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, usersResult.Error);
            }

            var unverifiedUsers = usersResult.Value
                .Where(u => u.VerificationStatus == UserVerificationStatus.PendingVerification)
                .Select(_dtoMapper.ToUserAccountResponseDto)
                .ToList();

            return StatusCode(StatusCodes.Status200OK, unverifiedUsers);
        }

        /// <summary>
        /// Verifies a user's account.
        /// </summary>
        /// <param name="userId">The ID of the user to verify.</param>
        /// <returns>A status indicating whether the verification was successful or not.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpPost("verifyUser")]
        public async Task<IActionResult> VerifyUser(int userId)
        {
            var userResult = await _userService.GetUserByIdAsync(userId);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, userResult.Error);
            }

            var user = userResult.Value;
            user.VerificationStatus = UserVerificationStatus.Verified;

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, updateResult.Error);
            }

            return StatusCode(StatusCodes.Status200OK, "User verified successfully.");
        }

        /// <summary>
        /// Denies a user's verification request.
        /// </summary>
        /// <param name="userId">The ID of the user whose verification is to be denied.</param>
        /// <returns>A status indicating whether the denial was successful or not.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpPost("denyUserVerification")]
        public async Task<IActionResult> DenyUserVerification(int userId)
        {
            var userResult = await _userService.GetUserByIdAsync(userId);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, userResult.Error);
            }

            var user = userResult.Value;
            user.VerificationStatus = UserVerificationStatus.NotVerified;

            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, updateResult.Error);
            }

            return StatusCode(StatusCodes.Status200OK, "User verification denied.");
        }

        /// <summary>
        /// Sends a friend request from the current user to another user.
        /// </summary>
        /// <param name="contactId">The ID of the user to whom the friend request is sent.</param>
        /// <returns>A status indicating whether the friend request was successfully sent or not.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 2)]
        [HttpPost("sendFriendRequest")]
        public async Task<IActionResult> SendFriendRequest(int contactId)
        {
            var userId = CurrentUser.Id;
            var result = await _userService.SendFriendRequestAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, result.Error);
            }

            return StatusCode(StatusCodes.Status200OK, "Friend request sent!");
        }

        /// <summary>
        /// Accepts a friend request for the current user.
        /// </summary>
        /// <param name="contactId">The ID of the user who sent the friend request.</param>
        /// <returns>A status indicating whether the friend request was successfully accepted or not.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 2)]
        [HttpPost("acceptFriendRequest")]
        public async Task<IActionResult> AcceptFriendRequest(int contactId)
        {
            var userId = CurrentUser.Id;
            var result = await _userService.AcceptFriendRequestAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, result.Error);
            }

            return StatusCode(StatusCodes.Status200OK, "Friend request accepted.");
        }

        /// <summary>
        /// Denies a friend request for the current user.
        /// </summary>
        /// <param name="contactId">The ID of the user who sent the friend request.</param>
        /// <returns>A status indicating whether the friend request was successfully denied or not.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 2)]
        [HttpPost("denyFriendRequest")]
        public async Task<IActionResult> DenyFriendRequest(int contactId)
        {
            var userId = CurrentUser.Id;
            var result = await _userService.DenyFriendRequestAsync(userId, contactId);

            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, result.Error);
            }

            return StatusCode(StatusCodes.Status200OK, "Friend request denied.");
        }

        /// <summary>
        /// Retrieves a list of pending friend requests for the current user.
        /// </summary>
        /// <returns>A list of pending friend requests if found; otherwise, an error message.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpGet("pendingFriendRequests")]
        public async Task<IActionResult> GetPendingFriendRequests()
        {
            var userId = CurrentUser.Id;
            var result = await _userService.GetPendingFriendRequestsAsync(userId);

            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }

            var pendingRequestsDto = result.Value.Select(_dtoMapper.ToUserContactResponseDto).ToList();

            return StatusCode(StatusCodes.Status200OK, pendingRequestsDto);
        }

        /// <summary>
        /// Deletes a user's account after verifying that there are no remaining funds in the user's wallets.
        /// </summary>
        /// <param name="id">The ID of the user whose account is to be deleted.</param>
        /// <returns>A status indicating whether the account deletion was successful or not.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 1)]
        [HttpDelete("deleteAccount/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userResult = await _userService.GetUserByIdAsync(id);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, userResult.Error);
            }

            var totalAmount = userResult.Value.Wallets.Select(w => w.Balance).Sum();
            if (totalAmount > 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "There are still funds in your wallets. Please withdraw all funds before deleting your account.");
            }

            var deleteResult = await _userService.DeleteUserAsync(id);

            if (!deleteResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, deleteResult.Error);
            }

            return StatusCode(StatusCodes.Status200OK, "Account deleted successfully.");
        }

        /// <summary>
        /// Retrieves all users for administrative purposes, with filtering options.
        /// </summary>
        /// <param name="userParameters">The query parameters for filtering users.</param>
        /// <returns>A list of users matching the filter criteria, along with the total user count.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 4)]
        [HttpGet("admin/users")]
        public async Task<IActionResult> AllUsers([FromQuery] UserQueryParameters userParameters)
        {
            var usersResult = await _userService.FilterUsersAsync(userParameters);
            var userDtos = usersResult.IsSuccess
                ? usersResult.Value.Select(_dtoMapper.ToUserAccountResponseDto).ToList()
                : Enumerable.Empty<UserAccountResponseDto>().ToList();

            var totalUserCountResult = await _userService.GetTotalUserCountAsync(userParameters);
            var totalUserCount = totalUserCountResult.IsSuccess ? totalUserCountResult.Value : 0;

            return StatusCode(StatusCodes.Status200OK, new
            {
                Users = userDtos,
                TotalUserCount = totalUserCount
            });
        }

        /// <summary>
        /// Retrieves all wallet transactions for administrative purposes, with filtering options.
        /// </summary>
        /// <param name="walletTransactionParameters">The query parameters for filtering wallet transactions.</param>
        /// <returns>A list of wallet transactions matching the filter criteria, along with the total transaction count.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 4)]
        [HttpGet("admin/walletTransactions")]
        public async Task<IActionResult> AllWalletTransactions([FromQuery] TransactionQueryParameters walletTransactionParameters)
        {
            var walletTransactionsResult = await _walletTransactionService.FilterWalletTransactionsAsync(walletTransactionParameters);
            var walletTransactionDtos = walletTransactionsResult.IsSuccess
                ? walletTransactionsResult.Value.Select(_dtoMapper.ToWalletTransactionDto).ToList()
                : Enumerable.Empty<WalletTransactionDto>().ToList();

            var totalWalletTransactionCountResult = await _walletTransactionService.GetTotalCountAsync(walletTransactionParameters);
            var totalWalletTransactionCount = totalWalletTransactionCountResult.IsSuccess ? totalWalletTransactionCountResult.Value : 0;

            return StatusCode(StatusCodes.Status200OK, new
            {
                WalletTransactions = walletTransactionDtos,
                TotalWalletTransactionCount = totalWalletTransactionCount
            });
        }

        /// <summary>
        /// Retrieves all card transactions for administrative purposes, with filtering options.
        /// </summary>
        /// <param name="cardTransactionParameters">The query parameters for filtering card transactions.</param>
        /// <returns>A list of card transactions matching the filter criteria, along with the total transaction count.</returns>
        [RequireAuthorization(minRequiredRoleLevel: 4)]
        [HttpGet("admin/cardTransactions")]
        public async Task<IActionResult> GetCardTransactions([FromQuery] CardTransactionQueryParameters cardTransactionParameters)
        {
            var cardTransactionsResult = await _cardTransactionService.FilterCardTransactionsAsync(cardTransactionParameters);
            var cardTransactionDtos = cardTransactionsResult.IsSuccess
                ? cardTransactionsResult.Value.Select(_dtoMapper.ToCardTransactionResponseDto).ToList()
                : Enumerable.Empty<CardTransactionResponseDto>().ToList();

            var totalCardTransactionCountResult = await _cardTransactionService.GetCardTransactionTotalCountAsync(cardTransactionParameters);
            var totalCardTransactionCount = totalCardTransactionCountResult.IsSuccess ? totalCardTransactionCountResult.Value : 0;

            return StatusCode(StatusCodes.Status200OK, new
            {
                CardTransactions = cardTransactionDtos,
                TotalCardTransactionCount = totalCardTransactionCount
            });
        }
    }
}
