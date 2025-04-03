using System.Net;
using System.Net.Mail;
using DTO;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication1;

namespace Bussines.MailServices;

public class MailService : IMailService
{
    private readonly ILogicSenderCong _logicSenderCong;
        // ("SmtpServer") 
        // ("Port") 
        // ("Password") 
        // ("FromEmail")
    private readonly SmtpSettings _configuration;
    private readonly ILogger<MailService> _logger;

    public MailService(ILogicSenderCong logicSenderCong, IOptions<SmtpSettings> smtpSettings , ILogger<MailService> logger)
    {  
        _logicSenderCong = logicSenderCong;
        _logger = logger;
        _configuration = smtpSettings.Value;
    }

    public async Task<bool> SengCongratulationsAsync()
    {
        _logger.LogInformation("Запуск отправки поздравлений");
        List<FriendDto> friendsList = await _logicSenderCong.GetTodayBirthdayAsync();
        if (friendsList == null || friendsList.Count == 0)
        {
            _logger.LogInformation("Сегодня нет именинников.");
            return false;
        }
        
        foreach (FriendDto friend in friendsList)
        {
            if(friend.FriendUsername!="james_taylor"){continue;}
            _logger.LogInformation($"Обработка поздравления для {friend.FriendUsername}");

            PozdrikIdDto? wishId = await _logicSenderCong.GetWishIdAsync(friend);
            string? congrTxt = string.Empty;
            if (wishId._pozdrikId == 0 || wishId._pozdrikId == null)
            {
                
                _logger.LogWarning($"Поздравление не найдено для {friend.FriendUsername}");
                congrTxt = "счастья";
            }
            else
            {

                congrTxt = await _logicSenderCong.GetCongrStrAsync(wishId._pozdrikId.Value);
            }

            string? email = await _logicSenderCong.GetEmailAsync(friend.AppId);
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning($"Не найден email для {friend.FriendUsername}");
                continue;
            }
            

            await SendMail(email, $"Поздравление - {friend.FriendUsername}", congrTxt);
        }

        return true;
    }

    private async Task<bool> SendMail(string to, string subject, string body)
    {
        try
        {
            using (SmtpClient client = new SmtpClient(_configuration.SmtpServer, _configuration.Port))
            {
                client.Credentials = new NetworkCredential(_configuration.FromEmail, _configuration.Password);
                client.EnableSsl = true;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration.FromEmail),
                    Subject = subject,
                    Body = body
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }

            _logger.LogInformation($"Письмо успешно отправлено на {to}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка отправки письма: {ex.Message}");
            return false;
        }
    }
}
