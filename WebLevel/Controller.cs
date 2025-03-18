using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1;
[ApiController]
[Route("api/Mail")]
public class Controller: ControllerBase
{
    private readonly IMailService _mailService;

    public Controller(IMailService mailService)
    {
        _mailService = mailService;
    }

    [HttpPost("send_congratulations")]
    public async Task<IActionResult> SendCongratulations()
    {
        bool resulr = await _mailService.SengCongratulationsAsync();
        if (resulr)
        {
            return Ok(new{message = "Congratulations successful"});
        }
        return BadRequest(new{message="Congratulations failed"});
    }
}