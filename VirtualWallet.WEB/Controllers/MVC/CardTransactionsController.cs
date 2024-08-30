﻿using Microsoft.AspNetCore.Mvc;
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
            var cardsResult = await _cardService.GetUserCardsAsync(CurrentUser.Id);
            if (!cardsResult.IsSuccess)
            {
                TempData["InfoMessage"] = "Currently, you do not have any cards. You will first need to add a card.";
                return RedirectToAction("AddCard", "Card");
            }

            await SetupViewBagForCardTransactionsAsync(filterParameters);

            var transactionViewModels = await GetTransactionViewModelsAsync(filterParameters);
            if (transactionViewModels == null)
            {
                TempData["ErrorMessage"] = "An error occurred while retrieving your transactions.";
                transactionViewModels = new List<CardTransactionViewModel>();
            }

            return View("~/Views/Card/CardTransactions.cshtml", transactionViewModels);
        }

        private async Task SetupViewBagForCardTransactionsAsync(CardTransactionQueryParameters filterParameters)
        {
            var totalCountResult = await _cardService.GetTotalCountAsync(filterParameters, CurrentUser.Id);
            ViewBag.TotalCount = totalCountResult.IsSuccess ? totalCountResult.Value : 0;
            ViewBag.PageSize = filterParameters.PageSize;
            ViewBag.PageNumber = filterParameters.PageNumber;
            ViewBag.FilterParameters = filterParameters;

            var cardsResult = await _cardService.GetUserCardsAsync(CurrentUser.Id);
            if (cardsResult.IsSuccess)
            {
                ViewBag.Cards = cardsResult.Value.Select(_viewModelMapper.ToCardViewModel).ToList();
            }
        }

        private async Task<List<CardTransactionViewModel>> GetTransactionViewModelsAsync(CardTransactionQueryParameters filterParameters)
        {
            var transactionsResult = await _cardService.FilterByAsync(filterParameters, CurrentUser.Id);
            if (transactionsResult.IsSuccess)
            {
                return transactionsResult.Value.Select(_viewModelMapper.ToCardTransactionViewModel).ToList();
            }
            return null;
        }

    }
}
