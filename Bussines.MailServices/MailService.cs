using DTO;
using Interfaces;

namespace Bussines.MailServices;

public class MailService:IMailService
{
    private readonly ILogicSenderCong _logicSenderCong;

    public MailService(ILogicSenderCong logicSenderCong)
    {
        _logicSenderCong = logicSenderCong;
    }
    public async Task<bool> SengCongratulationsAsync()
    {
        List<FriendDto> friendsList = await _logicSenderCong.GetTodayBirthdayAsync();
        if (friendsList == null || friendsList.Count == 0)
        {
            return false;
        }

        foreach (FriendDto friend in friendsList)
        {
            int? wishId = await _logicSenderCong.GetWishIdAsync(friend.AppId,friend.FriendUsername);
            if (wishId == null)
            {
                continue;
            }
            string congrTxt=await _logicSenderCong.GetCongrStrAsync(wishId.Value);
            string? email=await _logicSenderCong.GetEmailAsync(friend.AppId);
            if (string.IsNullOrEmpty(email)) continue;
            
        }
    }
    private void SendMail()
}