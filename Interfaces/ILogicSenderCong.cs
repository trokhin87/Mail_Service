using DTO;

namespace Interfaces;

public interface ILogicSenderCong
{
    Task<List<FriendDto>> GetTodayBirthdayAsync();
    Task<PozdrikIdDto?> GetWishIdAsync( FriendDto friendDto);
    Task<string> GetEmailAsync(Guid AppId);
    Task<string?> GetCongrStrAsync(int wishId);
}