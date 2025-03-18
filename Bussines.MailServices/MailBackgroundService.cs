using Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
namespace Bussines.MailServices;

public class MailBackgroundService: BackgroundService
{
    private readonly IMailService _mailService;
    public MailBackgroundService(IMailService mailService)
    {
        _mailService = mailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            TimeSpan timeToWait = GetNextRunTime();
            await Task.Delay(timeToWait, stoppingToken);

            try
            {
                bool result = await _mailService.SengCongratulationsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    private TimeSpan GetNextRunTime()
    {
        DateTime now = DateTime.Now;
        DateTime nextRun = now.Date.AddHours(9); // 9:00 утра

        if (now > nextRun) // Если уже позже 9 утра, ждем до следующего дня
        {
            nextRun = nextRun.AddDays(1);
        }

        return nextRun - now;
    }
}