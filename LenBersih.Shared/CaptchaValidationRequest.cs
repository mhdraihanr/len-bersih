using System.ComponentModel.DataAnnotations;

namespace LenBersih.Shared;

public class CaptchaValidationRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string Input { get; set; } = string.Empty;
}

public class CaptchaValidationResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}