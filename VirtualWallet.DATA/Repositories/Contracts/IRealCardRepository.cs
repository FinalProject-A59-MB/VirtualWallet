using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IRealCardRepository
    {
        Task<RealCard?> GetByPaymentProcessorTokenAsync(string paymentProcessorToken);

        Task<RealCard?> GetByCardNumberAsync(string cardNumber);

        Task UpdateRealCardAsync(RealCard realCard);
    }
}
