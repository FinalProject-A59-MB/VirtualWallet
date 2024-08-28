using System.Globalization;
using VirtualWallet.DATA.Models.Enums;

public class CardViewModel
{
    private string _cardNumber;
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string CardNumber
    {
        get => _cardNumber;
        set => _cardNumber = value?.Replace(" ", string.Empty);
    }

    private DateTime _expirationDate;
    public DateTime ExpirationDate
    {
        get => _expirationDate;
        set => _expirationDate = ConvertToDateTime(value);
    }

    public string ExpirationDateString
    {
        get => _expirationDate.ToString("MM/yy");
        set => _expirationDate = ConvertToDateTime(value);
    }

    public string CardHolderName { get; set; }
    public CardType CardType { get; set; }
    public string? Issuer { get; set; }
    public string Cvv { get; set; }
    public string? PaymentProcessorToken { get; set; }

    public CurrencyType Currency { get; set; }

    public string? CustomError { get; set; }

    // Method to convert "MM/YY" string to DateTime
    private DateTime ConvertToDateTime(string monthYear)
    {
        if (DateTime.TryParseExact(monthYear, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
        {
            // Returning the first day of the specified month and year
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }
        else
        {
            throw new FormatException("Invalid date format. Expected MM/YY.");
        }
    }

    // Overloaded method to accept DateTime directly
    private DateTime ConvertToDateTime(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }
}
