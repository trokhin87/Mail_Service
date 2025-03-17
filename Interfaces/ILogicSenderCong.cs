using DTO;

namespace Interfaces;

public interface ILogicSenderCong
{
    Task<List<FriendDto>> GetTodayBirthdayAsync();
    Task<int?> GetWishIdAsync(Guid userId, string friendUsername);
    Task<string> GetEmailAsync(Guid AppId);
    Task<string?> GetCongrStrAsync(int wishId);
}