using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IUserContactRepository
    {
        IEnumerable<UserContact> GetContactsByUserId(int userId);
        void AddContact(UserContact userContact);
        void RemoveContact(UserContact userContact);
    }
}
