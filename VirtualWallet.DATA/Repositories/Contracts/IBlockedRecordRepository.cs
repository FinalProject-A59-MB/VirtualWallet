using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IBlockedRecordRepository
    {
        IEnumerable<BlockedRecord> GetBlockedRecordsByUserId(int userId);
        BlockedRecord GetLatestBlockedRecordByUserId(int userId);
        void AddBlockedRecord(BlockedRecord blockedRecord);
    }
}
