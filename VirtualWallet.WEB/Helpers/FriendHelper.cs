using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Helpers
{
    public static class FriendHelper
    {
        public static bool AreFriends(int userId1, int userId2, ApplicationDbContext context)
        {
            if (userId1 == 0 || userId2 == 0) return false;

            var isUser1InUser2Contacts = context.UserContacts
                .Any(uc => uc.UserId == userId2 && uc.ContactId == userId1 && uc.Status == FriendRequestStatus.Accepted);

            // Check if userId2 is in userId1's contacts with an accepted status
            var isUser2InUser1Contacts = context.UserContacts
                .Any(uc => uc.UserId == userId1 && uc.ContactId == userId2 && uc.Status == FriendRequestStatus.Accepted);

            return isUser1InUser2Contacts && isUser2InUser1Contacts;
        }
    }


}
