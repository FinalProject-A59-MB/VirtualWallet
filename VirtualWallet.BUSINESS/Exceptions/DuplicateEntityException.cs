namespace VirtualWallet.BUSINESS.Exceptions;

public class DuplicateEntityException : Exception
{
    public DuplicateEntityException(string message) : base(message)
    {
    }
}
