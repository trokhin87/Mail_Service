using DTO;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Bussines.MailServices
{
    public class LogicSender : ILogicSenderCong
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public LogicSender(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ProxyMicroservice:BaseUrl"];
        }



        public async Task<string?> GetCongrStrAsync(int wishId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/mailbot/{wishId}");
            if(!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetEmailAsync(Guid AppId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/mailbot/email/{AppId}");
            if(!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

            public async Task<List<FriendDto>> GetTodayBirthdayAsync()
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/mailbot/birthdays/today");
                if(!response.IsSuccessStatusCode)
                {
                    return null;
                }
            return await response.Content.ReadFromJsonAsync<List<FriendDto>>() ?? new List<FriendDto>();
        }

        public async Task<int?> GetWishIdAsync(Guid userId, string friendUsername)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/mailbot/pozdrik/{friendUsername}/{userId}");
            if(!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<int?>();
        }
    }
}
