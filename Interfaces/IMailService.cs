namespace Interfaces;

public interface IMailService
{
    Task<bool> UpdateUserDataAsync();
}