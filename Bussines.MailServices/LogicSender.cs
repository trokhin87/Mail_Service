using DTO;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Bussines.MailServices;

public class LogicSender : ILogicSenderCong
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly ILogger<LogicSender> _logger;

    public LogicSender(HttpClient httpClient, IConfiguration configuration, ILogger<LogicSender> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["ProxyMicroservice:BaseUrl"];
    }

    public async Task<string?> GetCongrStrAsync(int wishId)
    {
        _logger.LogInformation($"Получение поздравления с ID {wishId}");
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/mailbot/GetCongrString/{wishId}");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Не удалось получить поздравление. Код: {response.StatusCode}");
            return null;
        }
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetEmailAsync(Guid AppId)
    {
        _logger.LogInformation($"Получение email для AppId: {AppId}");
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/mailbot/email/{AppId}");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Email не найден. Код: {response.StatusCode}");
            return null;
        }
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<List<FriendDto>> GetTodayBirthdayAsync()
    {
        _logger.LogInformation("Получение списка именинников на сегодня");
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/mailbot/birthdays/today");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Ошибка при получении списка. Код: {response.StatusCode}");
            return null;
        }
        return await response.Content.ReadFromJsonAsync<List<FriendDto>>() ?? new List<FriendDto>();
    }

    public async Task<PozdrikIdDto?> GetWishIdAsync(FriendDto friendDto)
    {
        _logger.LogInformation($"Получение ID поздравления для {friendDto.FriendUsername}");
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/mailbot/getPozdrikId", friendDto);
            
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Поздравление не найдено. Код: {response.StatusCode}");
            return null;
        }
        return await response.Content.ReadFromJsonAsync<PozdrikIdDto>();
    }
}
