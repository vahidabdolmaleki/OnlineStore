using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.Images;

public class UploadProductImageDto
{
    [Required(ErrorMessage = "فایل تصویر الزامی است.")]
    public IFormFile File { get; set; } = null!;

    public int? ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    public string? AltText { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsMain { get; set; } = false;
}
