using System.ComponentModel.DataAnnotations;

namespace LenBersih.Shared;

public class SecurityNumberModel
{
    [Required]
    [Compare(nameof(CaptchaText), ErrorMessage = "The entered Security Number is not correct.")]
    public string EnteredCaptchaText { set; get; } = string.Empty;

    public string CaptchaText { set; get; } = string.Empty;
}