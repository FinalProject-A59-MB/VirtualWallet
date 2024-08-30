using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.WEB.Controllers.MVC;
using VirtualWallet.WEB.Models.ViewModels.CardViewModels;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.DATA.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VirtualWallet.WEB.Controllers
{
    [RequireAuthorization(minRequiredRoleLevel: 1)]
    public class CardTransactionsController : BaseController
    {
        private readonly ICardTransactionService _cardTransactionService;
        private readonly ICardService _cardService;
        private readonly IViewModelMapper _viewModelMapper;

        public CardTransactionsController(
            ICardTransactionService cardTransactionService,
            ICardService cardService,
            IViewModelMapper viewModelMapper
            )
        {
            _cardTransactionService = cardTransactionService;
            _cardService = cardService;
            _viewModelMapper = viewModelMapper;
        }


        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] CardTransactionQueryParameters filterParameters)
        {

            var cards = await _cardService.GetUserCardsAsync(CurrentUser.Id);
            if (!cards.IsSuccess)
            {
                TempData["InfoMessage"] = "Currently you do not have any cards. You will first need to add a card.";
                return RedirectToAction("AddCard", "Card");
            }
            ViewBag.Cards = cards.Value.Select(_viewModelMapper.ToCardViewModel).ToList();
            var totalCount = await _cardService.GetTotalCountAsync(filterParameters, CurrentUser.Id);

            ViewBag.TotalCount = totalCount.Value;
            ViewBag.PageSize = filterParameters.PageSize;
            ViewBag.PageNumber = filterParameters.PageNumber;
            ViewBag.FilterParameters = filterParameters;

            var transactions = await _cardService.FilterByAsync(filterParameters,CurrentUser.Id);
            if (!transactions.IsSuccess)
            {
                var transactionViewModels2 = new List<CardTransactionViewModel>();
                return View(transactionViewModels2);
            }

            var transactionViewModels = transactions.Value.Select(_viewModelMapper.ToCardTransactionViewModel).ToList();



            return View("~/Views/Card/CardTransactions.cshtml", transactionViewModels);
        }
    }
}
