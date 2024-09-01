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
    public string ExpirationDate { get; set; }

    public string CardHolderName { get; set; }
    public CardType CardType { get; set; }
    public string? Issuer { get; set; }
    public string Cvv { get; set; }
    public string? PaymentProcessorToken { get; set; }

    public CurrencyType Currency { get; set; }

    public string? CustomError { get; set; }

}
