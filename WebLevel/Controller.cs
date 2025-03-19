using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApplication1;

[ApiController]
[Route("api/Mail")]
public class Controller : ControllerBase
{
    private readonly IMailService _mailService;

    public Controller(IMailService mailService)
    {
        _mailService = mailService;
    }

    /// <summary>
    /// Отправляет поздравления.
    /// </summary>
    [HttpPost("send_congratulations")]
    [SwaggerOperation(Summary = "Отправить поздравления", Description = "Отправляет поздравления пользователям." +
                                                                        "\n\ninsert into mail_comprehensions(appid, mail)" +
                                                                        "\nvalues('9f5b4887-b170-4d11-b0a5-93cc285bd2bb', 'gl.krutoi@mail.ru')")]
    [SwaggerResponse(200, "Поздравления успешно отправлены", typeof(object))]
    [SwaggerResponse(400, "Ошибка при отправке поздравлений", typeof(object))]
    public async Task<IActionResult> SendCongratulations()
    {
        bool result = await _mailService.SengCongratulationsAsync();
        if (result)
        {
            return Ok(new { message = "Congratulations successful" });
        }
        return BadRequest(new { message = "Congratulations failed" });
    }
}