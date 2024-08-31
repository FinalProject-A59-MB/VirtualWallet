using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.TESTS.BUSINESS.Services.MockRepositories
{
    public class MockCardTransactionRepository
    {
        public Mock<ICardTransactionRepository> GetMockRepository()
        {
            var mockRepository = new Mock<ICardTransactionRepository>();

            var sampleCardTransactions = new List<CardTransaction> { TestHelper.GetTestCardTransaction() };

            mockRepository.Setup(x => x.GetTransactionsByUserId(It.IsAny<int>()))
                          .Returns((int userId) => sampleCardTransactions.Where(t => t.UserId == userId).AsQueryable());

            mockRepository.Setup(x => x.GetTransactionsByCardId(It.IsAny<int>()))
                          .Returns((int cardId) => sampleCardTransactions.Where(t => t.CardId == cardId).AsQueryable());

            mockRepository.Setup(x => x.GetTransactionByIdAsync(It.IsAny<int>()))
                          .ReturnsAsync((int id) => sampleCardTransactions.FirstOrDefault(t => t.Id == id));

            mockRepository.Setup(x => x.AddCardTransactionAsync(It.IsAny<CardTransaction>()))
                          .Callback((CardTransaction transaction) => sampleCardTransactions.Add(transaction))
                          .Returns(Task.CompletedTask);

            mockRepository.Setup(x => x.GetAllCardTransactionsAsync())
                          .ReturnsAsync(sampleCardTransactions);

            return mockRepository;
        }
    }
}
