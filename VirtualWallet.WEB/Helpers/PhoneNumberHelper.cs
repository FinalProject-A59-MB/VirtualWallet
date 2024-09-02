namespace VirtualWallet.WEB.Helpers
{
    public static class PhoneNumberHelper
    {
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.Length != 10)
            {
                // Handle the case where the phone number doesn't have 10 digits
                return phoneNumber; // or throw an exception, or return some other default value
            }

            var areaCode = phoneNumber.Substring(0, 1);
            var firstPart = phoneNumber.Substring(1, 3);
            var secondPart = phoneNumber.Substring(4, 3);
            var lastPart = phoneNumber.Substring(7, 3);

            return $"(+{areaCode}) {firstPart} {secondPart} {lastPart}";
        }
    }
}
