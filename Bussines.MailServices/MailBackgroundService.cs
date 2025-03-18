using Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bussines.MailServices;

public class MailBackgroundService : BackgroundService
{
    private readonly IMailService _mailService;
    private readonly ILogger<MailBackgroundService> _logger;

    public MailBackgroundService(IMailService mailService, ILogger<MailBackgroundService> logger)
    {
        _mailService = mailService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            TimeSpan timeToWait = GetNextRunTime();
            _logger.LogInformation($"Следующая отправка писем через {timeToWait}");

            await Task.Delay(timeToWait, stoppingToken);

            try
            {
                bool result = await _mailService.SengCongratulationsAsync();
                _logger.LogInformation(result
                    ? "Поздравления успешно отправлены."
                    : "Нет именинников для поздравления.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при отправке писем: {ex.Message}");
            }
        }
    }

    private TimeSpan GetNextRunTime()
    {
        DateTime now = DateTime.Now;
        DateTime nextRun = now.Date.AddHours(9);

        if (now > nextRun)
        {
            nextRun = nextRun.AddDays(1);
        }

        return nextRun - now;
    }
}