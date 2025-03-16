using DTO;

namespace Interfaces;

public interface ILogic
{
    Task<List<FriendDto>> GetTodayBirthdayAsync();
    Task<int?> GetWishIdAsync(Guid userId, string friendUsername);
     
}