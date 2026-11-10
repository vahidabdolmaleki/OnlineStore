using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.ProductColors;

public class UpdateProductColorDto
{
    [Required(ErrorMessage = "نام رنگ الزامی است.")]
    [StringLength(50, ErrorMessage = "نام رنگ نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")]
    public string ColorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "کد رنگ الزامی است.")]
    [StringLength(20, ErrorMessage = "کد رنگ نمی‌تواند بیشتر از ۲۰ کاراکتر باشد.")]
    public string ColorCode { get; set; } = string.Empty;
}
