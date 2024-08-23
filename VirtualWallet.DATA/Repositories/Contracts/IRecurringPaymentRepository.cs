using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IRecurringPaymentRepository
    {
        IEnumerable<RecurringPayment> GetPaymentsByUserId(int userId);
        RecurringPayment GetPaymentById(int id);
        void AddRecurringPayment(RecurringPayment recurringPayment);
        void UpdateRecurringPayment(RecurringPayment recurringPayment);
        void RemoveRecurringPayment(int id);
    }
}
