namespace VirtualWallet.TESTS.BUSINESS.Services.MockRepositories
{
    using global::VirtualWallet.DATA.Models.Enums;
    using global::VirtualWallet.DATA.Models;
    using global::VirtualWallet.DATA.Repositories.Contracts;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    namespace VirtualWallet.TESTS.BUSINESS.Services.MockRepositories
    {
        public class MockCardRepository
        {
            public Mock<ICardRepository> GetMockRepository()
            {
                var mockRepository = new Mock<ICardRepository>();

                var sampleCards = new List<Card> { TestHelper.GetTestCard() };
                var sampleCardTransactions = new List<CardTransaction> { TestHelper.GetTestCardTransaction() };

                mockRepository.Setup(x => x.GetCardsByUserId(It.IsAny<int>()))
                              .Returns((int userId) => sampleCards.Where(card => card.UserId == userId).AsQueryable());

                mockRepository.Setup(x => x.GetCardByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync((int id) => sampleCards.FirstOrDefault(card => card.Id == id));

                mockRepository.Setup(x => x.AddCardAsync(It.IsAny<Card>()))
                              .Callback((Card card) => sampleCards.Add(card))
                              .Returns(Task.CompletedTask);

                mockRepository.Setup(x => x.UpdateCardAsync(It.IsAny<Card>()))
                              .Callback((Card card) =>
                              {
                                  var existingCard = sampleCards.FirstOrDefault(c => c.Id == card.Id);
                                  if (existingCard != null)
                                  {
                                      existingCard.CardNumber = card.CardNumber;
                                      existingCard.ExpirationDate = card.ExpirationDate;
                                      existingCard.CardHolderName = card.CardHolderName;
                                      existingCard.Currency = card.Currency;
                                      existingCard.UserId = card.UserId;
                                  }
                              })
                              .Returns(Task.CompletedTask);

                mockRepository.Setup(x => x.RemoveCardAsync(It.IsAny<int>()))
                              .Callback((int id) =>
                              {
                                  var cardToRemove = sampleCards.FirstOrDefault(c => c.Id == id);
                                  if (cardToRemove != null)
                                  {
                                      sampleCards.Remove(cardToRemove);
                                      sampleCardTransactions.RemoveAll(t => t.CardId == id);
                                  }
                              })
                              .Returns(Task.CompletedTask);

                mockRepository.Setup(x => x.FilterByAsync(It.IsAny<CardTransactionQueryParameters>(), It.IsAny<int?>()))
                              .ReturnsAsync((CardTransactionQueryParameters filterParams, int? userId) =>
                              {
                                  var query = sampleCardTransactions.AsQueryable();

                                  if (userId.HasValue)
                                  {
                                      query = query.Where(t => t.UserId == userId.Value);
                                  }

                                  if (filterParams.CardId != 0)
                                  {
                                      query = query.Where(t => t.CardId == filterParams.CardId);
                                  }

                                  if (filterParams.Amount != 0)
                                  {
                                      query = query.Where(t => t.Amount == filterParams.Amount);
                                  }

                                  if (filterParams.CreatedAfter.HasValue)
                                  {
                                      query = query.Where(t => t.CreatedAt >= filterParams.CreatedAfter.Value);
                                  }

                                  if (filterParams.CreatedBefore.HasValue)
                                  {
                                      query = query.Where(t => t.CreatedAt <= filterParams.CreatedBefore.Value);
                                  }

                                  if (!string.IsNullOrEmpty(filterParams.TransactionType))
                                  {
                                      if (Enum.TryParse<TransactionType>(filterParams.TransactionType, out var transactionTypeEnum))
                                      {
                                          query = query.Where(t => t.TransactionType == transactionTypeEnum);
                                      }
                                  }

                                  return query.ToList();
                              });

                mockRepository.Setup(x => x.GetTotalCountAsync(It.IsAny<CardTransactionQueryParameters>(), It.IsAny<int?>()))
                              .ReturnsAsync((CardTransactionQueryParameters filterParams, int? userId) =>
                              {
                                  var query = sampleCardTransactions.AsQueryable();

                                  if (userId.HasValue)
                                  {
                                      query = query.Where(t => t.UserId == userId.Value);
                                  }

                                  if (filterParams.CardId != 0)
                                  {
                                      query = query.Where(t => t.CardId == filterParams.CardId);
                                  }

                                  if (filterParams.CreatedAfter.HasValue)
                                  {
                                      query = query.Where(t => t.CreatedAt >= filterParams.CreatedAfter.Value);
                                  }

                                  if (filterParams.CreatedBefore.HasValue)
                                  {
                                      query = query.Where(t => t.CreatedAt <= filterParams.CreatedBefore.Value);
                                  }

                                  if (!string.IsNullOrEmpty(filterParams.TransactionType))
                                  {
                                      if (Enum.TryParse<TransactionType>(filterParams.TransactionType, out var transactionTypeEnum))
                                      {
                                          query = query.Where(t => t.TransactionType == transactionTypeEnum);
                                      }
                                  }

                                  return query.Count();
                              });

                return mockRepository;
            }
        }

    }
}

