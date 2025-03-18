using System.Net;
using System.Net.Mail;
using DTO;
using Interfaces;
using Microsoft.Extensions.Configuration;

namespace Bussines.MailServices;

public class MailService:IMailService
{
    private readonly ILogicSenderCong _logicSenderCong;
    private readonly string SMTPServer;
    private readonly int SMTPPort;
    private readonly string emailPassword;
    private readonly string emailFrom;
    public MailService(ILogicSenderCong logicSenderCong,IConfiguration configuration)
    {  
        _logicSenderCong = logicSenderCong;
        SMTPPort = 587;
        SMTPServer = configuration["EmailSettings:SmtpServer"];
        emailPassword = configuration["EmailSettings:Password"];
        emailFrom = configuration["EmailSettings:FromEmail"];
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
            await SendMail(email,$"Поздравление - {friend.FriendUsername}", congrTxt);
        }

        return true;
    }

    private async Task<bool> SendMail(string to, string subject, string body)
    {
        try
        {
            using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
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

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки письма: {ex.Message}");
            return false;
        }
    }
}