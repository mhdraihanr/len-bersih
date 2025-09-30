using Microsoft.AspNetCore.Mvc;
using LenBersih.Shared;
using DNTCaptcha.Core;

namespace LenBersih.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CaptchaController : ControllerBase
{
    private readonly IDNTCaptchaValidatorService _validatorService;

    public CaptchaController(IDNTCaptchaValidatorService validatorService)
    {
        _validatorService = validatorService;
    }

    [HttpPost("validate")]
    public ActionResult<CaptchaValidationResponse> ValidateCaptcha([FromBody] CaptchaValidationRequest request)
    {
        if (request is null)
        {
            return BadRequest("Request payload is required.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        bool isValid = _validatorService.HasRequestValidCaptchaEntry();

        return Ok(new CaptchaValidationResponse
        {
            IsValid = isValid,
            Message = isValid ? "CAPTCHA verification successful." : "CAPTCHA verification failed."
        });
    }
}