using System.Net;
using System.Net.Mail;
using DTO;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bussines.MailServices;

public class MailService : IMailService
{
    private readonly ILogicSenderCong _logicSenderCong;
    private readonly string SMTPServer;
    private readonly int SMTPPort;
    private readonly string emailPassword;
    private readonly string emailFrom;
    private readonly ILogger<MailService> _logger;

    public MailService(ILogicSenderCong logicSenderCong, IConfiguration configuration, ILogger<MailService> logger)
    {  
        _logicSenderCong = logicSenderCong;
        _logger = logger;
        SMTPPort = 587;
        SMTPServer = configuration["EmailSettings:SmtpServer"];
        emailPassword = configuration["EmailSettings:Password"];
        emailFrom = configuration["EmailSettings:FromEmail"];
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

            int? wishId = await _logicSenderCong.GetWishIdAsync(friend);
            string congrTxt=string.Empty;
            if (wishId == 0)
            {
                
                _logger.LogWarning($"Поздравление не найдено для {friend.FriendUsername}");
                congrTxt = "счастья";
            }
            else
            {

                congrTxt = await _logicSenderCong.GetCongrStrAsync(wishId.Value);
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
            using (SmtpClient client = new SmtpClient(SMTPServer, SMTPPort))
            {
                client.Credentials = new NetworkCredential(emailFrom, emailPassword);
                client.EnableSsl = true;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(emailFrom),
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
