using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.DATA.Repositories
{
    public class RealCardRepository : IRealCardRepository
    {
        private readonly ApplicationDbContext _context;

        public RealCardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RealCard?> GetByPaymentProcessorTokenAsync(string paymentProcessorToken)
        {
            return await _context.RealCards
                .FirstOrDefaultAsync(rc => rc.PaymentProcessorToken == paymentProcessorToken);
        }

        public async Task<RealCard?> GetByCardNumberAsync(string cardNumber)
        {
            return await _context.RealCards
                .FirstOrDefaultAsync(rc => rc.CardNumber == cardNumber);
        }

        public async Task UpdateRealCardAsync(RealCard realCard)
        {
            _context.RealCards.Update(realCard);
            await _context.SaveChangesAsync();
        }
    }
}
