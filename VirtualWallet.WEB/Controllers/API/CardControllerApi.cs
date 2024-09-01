using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.DTOs.CardDTOs;

namespace VirtualWallet.WEB.Controllers.API
{
    /// <summary>
    /// Controller responsible for managing cards, including adding, deleting, and handling transactions.
    /// </summary>
    [Route("api/Card")]
    [ApiController]
    [RequireAuthorization(minRequiredRoleLevel: 2)]
    public class CardControllerApi : BaseControllerApi
    {
        private readonly ICardService _cardService;
        private readonly IPaymentProcessorService _paymentProcessorService;
        private readonly IWalletService _walletService;
        private readonly ICardTransactionService _cardTransactionService;
        private readonly IDtoMapper _dtoMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardControllerApi"/> class.
        /// </summary>
        /// <param name="cardService">Service for managing cards.</param>
        /// <param name="paymentProcessorService">Service for processing payments.</param>
        /// <param name="walletService">Service for managing wallets.</param>
        /// <param name="cardTransactionService">Service for managing card transactions.</param>
        /// <param name="dtoMapper">Service for mapping DTOs to models.</param>
        public CardControllerApi(
            ICardService cardService,
            IPaymentProcessorService paymentProcessorService,
            IWalletService walletService,
            ICardTransactionService cardTransactionService,
            IDtoMapper dtoMapper)
        {
            _cardService = cardService;
            _paymentProcessorService = paymentProcessorService;
            _walletService = walletService;
            _cardTransactionService = cardTransactionService;
            _dtoMapper = dtoMapper;
        }

        /// <summary>
        /// Adds a new card for the current user.
        /// </summary>
        /// <param name="model">The card details to be added.</param>
        /// <returns>The added card details if successful; otherwise, an error message.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddCard([FromBody] CardRequestDto model)
        {
            var card = _dtoMapper.ToCard(model);

            var tokenResult = await _paymentProcessorService.VerifyAndRetrieveTokenAsync(card);

            if (!tokenResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, tokenResult.Error);
            }

            var currencyResult = await _paymentProcessorService.GetCardCurrency(tokenResult.Value);
            if (!currencyResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, currencyResult.Error);
            }

            card.PaymentProcessorToken = tokenResult.Value;
            card.Currency = currencyResult.Value;

            var addCardResult = await _cardService.AddCardAsync(CurrentUser, card);

            if (!addCardResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, addCardResult.Error);
            }

            var cardResponseDto = _dtoMapper.ToCardResponseDto(card);
            return StatusCode(StatusCodes.Status201Created, cardResponseDto);
        }

        /// <summary>
        /// Deletes a card by its ID.
        /// </summary>
        /// <param name="cardId">The ID of the card to delete.</param>
        /// <returns>A status indicating whether the deletion was successful or not.</returns>
        [HttpDelete("delete/{cardId}")]
        public async Task<IActionResult> DeleteCard(int cardId)
        {
            var cardResult = await _cardService.GetCardByIdAsync(cardId);

            if (!cardResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, cardResult.Error);
            }

            var deleteResult = await _cardService.DeleteCardAsync(cardId);

            if (!deleteResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, deleteResult.Error);
            }

            return StatusCode(StatusCodes.Status204NoContent);
        }

        /// <summary>
        /// Retrieves a card by its ID.
        /// </summary>
        /// <param name="cardId">The ID of the card to retrieve.</param>
        /// <returns>The card details if found; otherwise, an error message.</returns>
        [HttpGet("{cardId}")]
        public async Task<IActionResult> GetCardById(int cardId)
        {
            var cardResult = await _cardService.GetCardByIdAsync(cardId);

            if (!cardResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, cardResult.Error);
            }

            var cardDto = _dtoMapper.ToCardResponseDto(cardResult.Value);

            return StatusCode(StatusCodes.Status200OK, cardDto);
        }

        /// <summary>
        /// Deposits an amount from a card to a wallet.
        /// </summary>
        /// <param name="model">The transaction details, including card ID, wallet ID, and amount to deposit.</param>
        /// <returns>The transaction details if successful; otherwise, an error message.</returns>
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] CardTransactionRequestDto model)
        {
            var cardResult = await _cardService.GetCardByIdAsync(model.CardId);
            if (!cardResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, cardResult.Error);
            }

            var walletResult = await _walletService.GetWalletByIdAsync(model.WalletId);
            if (!walletResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, walletResult.Error);
            }

            var depositResult = await _cardTransactionService.DepositAsync(model.CardId, model.WalletId, model.Amount);
            if (!depositResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, depositResult.Error);
            }

            var transactionResponseDto = _dtoMapper.ToCardTransactionResponseDto(depositResult.Value);
            return StatusCode(StatusCodes.Status201Created, transactionResponseDto);
        }

        /// <summary>
        /// Withdraws an amount from a wallet to a card.
        /// </summary>
        /// <param name="model">The transaction details, including card ID, wallet ID, and amount to withdraw.</param>
        /// <returns>The transaction details if successful; otherwise, an error message.</returns>
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] CardTransactionRequestDto model)
        {
            var cardResult = await _cardService.GetCardByIdAsync(model.CardId);
            if (!cardResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, cardResult.Error);
            }

            var walletResult = await _walletService.GetWalletByIdAsync(model.WalletId);
            if (!walletResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, walletResult.Error);
            }

            var withdrawResult = await _cardTransactionService.WithdrawAsync(model.WalletId, model.CardId, model.Amount);
            if (!withdrawResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, withdrawResult.Error);
            }

            var transactionResponseDto = _dtoMapper.ToCardTransactionResponseDto(withdrawResult.Value);
            return StatusCode(StatusCodes.Status201Created, transactionResponseDto);
        }

        /// <summary>
        /// Retrieves all transactions for the current user's cards based on the specified filter parameters.
        /// </summary>
        /// <param name="filterParameters">The filter parameters for the transactions.</param>
        /// <returns>A list of transactions matching the filter parameters if found; otherwise, an error message.</returns>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetCardTransactions([FromQuery] CardTransactionQueryParameters filterParameters)
        {
            var cardsResult = await _cardService.GetUserCardsAsync(CurrentUser.Id);

            if (!cardsResult.IsSuccess || !cardsResult.Value.Any())
            {
                return StatusCode(StatusCodes.Status404NotFound, "No cards found.");
            }

            var transactionsResult = await _cardService.FilterByAsync(filterParameters, CurrentUser.Id);

            if (!transactionsResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, transactionsResult.Error);
            }

            var transactionsDto = transactionsResult.Value.Select(_dtoMapper.ToCardTransactionResponseDto).ToList();

            return StatusCode(StatusCodes.Status200OK, new { Transactions = transactionsDto });
        }
    }
}
