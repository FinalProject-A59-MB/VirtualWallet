using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.Transactions;
using Twilio.Jwt.AccessToken;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.ViewModels.CardViewModels;
using VirtualWallet.WEB.Models.ViewModels.UserViewModels;
using VirtualWallet.WEB.Models.ViewModels.WalletTransactionViewModels;
using VirtualWallet.WEB.Models.ViewModels.WalletViewModels;

namespace VirtualWallet.WEB.Controllers.MVC
{
    [RequireAuthorization(minRequiredRoleLevel: 2)]
    public class WalletTransactionsController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IWalletService _walletService;
        private readonly IViewModelMapper _viewModelMapper;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public WalletTransactionsController(IWalletTransactionService walletTransactionService,
            IWalletService walletService,
            IViewModelMapper viewModelMapper,
            IEmailService emailService,
            IUserService userService)
        {
            _walletTransactionService = walletTransactionService;
            _walletService = walletService;
            _viewModelMapper = viewModelMapper;
            _emailService = emailService;
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _walletTransactionService.GetTransactionByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            return View(_viewModelMapper.ToWalletTransactionViewModel(result.Value));
        }

        [HttpGet]
        public async Task<IActionResult> VerifyPayment(SendMoneyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var recepientResult = await _userService.GetUserByIdAsync(model.RecipientId);
            if (!recepientResult.IsSuccess)
            {
                TempData["ErrorMessage"] = recepientResult.Error;
                return RedirectToAction("Wallets", "User");
            }

            var result = await _walletTransactionService.VerifySendAmountAsync(model.SenderWalletId, recepientResult.Value, model.Amount);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            TempData["SuccessMessage"] = "A verification code has been sent to your email. Please use it to verify the transaction to continue.";

            return View(_viewModelMapper.ToWalletTransactionViewModel(result.Value));
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPayment(WalletTransactionViewModel transaction)
        {

            var result = await _walletTransactionService.GetTransactionByIdAsync(transaction.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            if (transaction.VerificationCode != result.Value.VerificationCode)
            {
                TempData["ErrorMessage"] = "Incorrect code";
                return RedirectToAction("VerifyPayment", new { id = transaction.Id });
            }
            var resultDeposit = await _walletTransactionService.ProcessSendAmountAsync(_viewModelMapper.ToWalletTransaction(transaction));

            //TODO CHECK RESULT
            if (!resultDeposit.IsSuccess)
            {
                TempData["ErrorMessage"] = resultDeposit.Error;
                return RedirectToAction("Wallets", "User");
            }

            TempData["SuccessMessage"] = "Transaction completed succesfully.";

            return RedirectToAction("Wallets","User");
        }

        [HttpGet]
        public async Task<IActionResult> DepositInternally()
        {
            var result = await _walletService.GetWalletsByUserIdAsync(CurrentUser.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            var vm = new SendMoneyViewModel()
            {
                From = result.Value.Select(x => _viewModelMapper.ToWalletViewModel(x)),
            };

            return View(vm);
        }

        [RequireAuthorization(minRequiredRoleLevel: 3)]
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            if (!CurrentUser.Wallets.Any())
            {
                TempData["InfoMessage"] = "Currently you do not have any wallets. You will first need to add a wallet and add some funds to it to use this service.";
                return RedirectToAction("AddCard", "Card");
            }

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

            IEnumerable<UserViewModel> userViewModels = result.Value.Where(u => u.Id != CurrentUser.Id).Select(_viewModelMapper.ToUserViewModel);

            return View(userViewModels);
        }




        [HttpGet]
        public async Task<IActionResult> SendTo(int recipientId)
        {
            var wallets = await _walletService.GetWalletsByUserIdAsync(CurrentUser.Id);

            if (!wallets.IsSuccess)
            {
                TempData["ErrorMessage"] = wallets.Error;
                return RedirectToAction("Index", "Home");
            }
            var recipient = await _userService.GetUserByIdAsync(recipientId);
            if (!wallets.IsSuccess)
            {
                TempData["ErrorMessage"] = recipient.Error;
                return RedirectToAction("Index", "Home");
            }
            var viewModel = new SendMoneyViewModel
            {
                From = wallets.Value.Select(_viewModelMapper.ToWalletViewModel),
                RecipientId = recipientId,
                RecipientName = recipient.Value.Username,
                RecipientEmail = recipient.Value.Email,
            };

            return View(viewModel);
        }

        [HttpPost]
        [RequireAuthorization(minRequiredRoleLevel: 3)]
        public async Task<IActionResult> SendConfirm(SendMoneyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("TransactionConfirmation", "WalletTransactions");
        }

        [HttpGet]
        public IActionResult RequestDeposit()
        {
            var activeContacts = CurrentUser.Contacts.FirstOrDefault(c=>c.Status== FriendRequestStatus.Accepted);
            if (activeContacts==null)
            {
                TempData["InfoMessage"] = "Currently you do not have any active contacts. You need to add people to your contact list before you can be able to request funds.";
                return RedirectToAction("Profile", "User");
            }
            var model = new RequestMoneyViewModel { 
                Contacts = CurrentUser.Contacts,
                SenderId = CurrentUser.Id
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult RequestConfirm(RequestMoneyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Contacts = CurrentUser.Contacts.Where(c => c.Status == FriendRequestStatus.Accepted).ToList();
                return View(model);
            }

            var activeContacts = CurrentUser.Contacts.FirstOrDefault(c => c.Status == FriendRequestStatus.Accepted);
            if (activeContacts == null)
            {
                TempData["InfoMessage"] = "Currently you do not have any active contacts. You need to add people to your contact list before you can be able to request funds.";
                return RedirectToAction("Profile", "User");
            }

            model.Contacts = CurrentUser.Contacts.Where(c => c.Status == FriendRequestStatus.Accepted).ToList();

            return View("RequestConfirm", model);
        }

        [HttpPost]
        public IActionResult RequestSend(RequestMoneyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Contacts = CurrentUser.Contacts.Where(c => c.Status == FriendRequestStatus.Accepted).ToList();
                return View("RequestConfirm", model);
            }

            //// Process the request
            //var transaction = new WalletTransactionRequest
            //{
            //    SenderId = model.SenderId,
            //    AmountSent = model.Amount,
            //    Description = model.Description,
            //    Status = TransactionStatus.Pending,
            //    CreatedAt = DateTime.UtcNow
            //};

            //var result = _walletTransactionService.CreateTransaction(transaction);

            //if (!result.IsSuccess)
            //{
            //    TempData["ErrorMessage"] = "There was an error processing your request. Please try again.";
            //    return RedirectToAction("RequestDeposit");
            //}

            TempData["SuccessMessage"] = "Your request has been successfully sent.";
            return RedirectToAction("Index", "WalletTransactions");
        }




        [HttpGet]
        public async Task<IActionResult> DepositConfirm(SendMoneyViewModel model)
        {
            
            if (!ModelState.IsValid)
            {

                return View(model);
            }

            model.From = CurrentUser.Wallets.Select(x => _viewModelMapper.ToWalletViewModel(x));

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(SendMoneyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _walletTransactionService.ProcessSendAmountInternalAsync(model.SenderWalletId, model.RecipientId, model.Amount);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("DepositInternaly","WalletTransations",model);
            }

            TempData["SuccessMessage"] = "The transaction was a success.";
            return RedirectToAction("Wallets", "User");
        }

        [HttpPost]
        public async Task<int> RequestDeposit(int senderWalletId, decimal amount, string description)
        {
            throw new NotSupportedException();
            //var myMainWalletIdResult = await _walletService.GetWalletIdByUserDetailsAsync(CurrentUser.Username);

            //var result = await _walletTransactionService.DepositStep1Async(senderWalletId, myMainWalletIdResult.Value, amount);

            //if (!result.IsSuccess)
            //{
            //    TempData["ErrorMessage"] = result.Error;
            //}

            //var senderWalletResult = await _walletService.GetWalletByIdAsync(senderWalletId);

            //if (!senderWalletResult.IsSuccess)
            //{
            //    TempData["ErrorMessage"] = result.Error;
            //}

            //var senderEmail = senderWalletResult.Value?.User?.Email;


            //if (string.IsNullOrEmpty(senderEmail))
            //{
            //    TempData["ErrorMessage"] = "Incorrect user information.";
            //}

            //string transactionUrl = Url.Action("VerifyPayment", "WalletTransaction", new { id = result.Value }, Request.Scheme);
            //string anchorTag = $"<a href=\"{transactionUrl}\">View Transaction</a>";

            //await _emailService.SendEmailAsync(senderEmail, $"{CurrentUser.Username} is requesting payment", $"{CurrentUser.Username} sent you a payment request for {amount} BGN with a description: {description}. Please confirm the payment at {anchorTag}");

            //return result.Value;
        }


        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] WalletTransactionQueryParameters filterParameters)
        {
            await SetupViewBagForWalletTransactionsAsync(filterParameters);

            List<WalletTransactionViewModel> transactionViewModels = await GetTransactionViewModelsAsync(filterParameters);
            if (transactionViewModels == null || !transactionViewModels.Any())
            {
                TempData["InfoMessage"] = "No transactions found with the current parameters";
                transactionViewModels = new List<WalletTransactionViewModel>();
            }

            return View(transactionViewModels);
        }

        private async Task SetupViewBagForWalletTransactionsAsync(WalletTransactionQueryParameters filterParameters)
        {
            var totalCountResult = await _walletService.GetTotalWalletTransactionCountAsync(filterParameters, CurrentUser.Id);
            ViewBag.TotalCount = totalCountResult.IsSuccess ? totalCountResult.Value : 0;
            ViewBag.PageSize = filterParameters.PageSize;
            ViewBag.PageNumber = filterParameters.PageNumber;
            ViewBag.FilterParameters = filterParameters;

            var walletsResult = await _walletService.GetWalletsByUserIdAsync(CurrentUser.Id);
            if (walletsResult.IsSuccess)
            {
                ViewBag.UserWallets = walletsResult.Value.Select(_viewModelMapper.ToWalletViewModel).ToList();
            }

            ViewBag.CurrentWalletId = filterParameters.WalletId;
        }

        private async Task<List<WalletTransactionViewModel>> GetTransactionViewModelsAsync(WalletTransactionQueryParameters filterParameters)
        {
            var transactionsResult = await _walletService.GetWalletTransactionsAsync(filterParameters, CurrentUser.Id);
            if (transactionsResult.IsSuccess)
            {
                return transactionsResult.Value.Select(_viewModelMapper.ToWalletTransactionViewModel).ToList();
            }
            return null;
        }

    }
}
