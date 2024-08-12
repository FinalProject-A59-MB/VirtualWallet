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
        RealCard GetRealCardById(int id);
        IEnumerable<RealCard> GetAllRealCards();
        void AddRealCard(RealCard realCard);
        void UpdateRealCard(RealCard realCard);
    }
}
