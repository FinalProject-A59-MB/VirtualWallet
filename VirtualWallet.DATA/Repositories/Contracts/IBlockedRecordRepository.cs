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
