using System.ComponentModel.DataAnnotations;
using Shop.Domain.Enums;

namespace Shop.Application.DTOs.ProductRelations;

public class CreateProductRelationDto
{
    [Required(ErrorMessage = "شناسه محصول اصلی الزامی است.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "شناسه محصول مرتبط الزامی است.")]
    public int RelatedProductId { get; set; }

    [Required(ErrorMessage = "نوع ارتباط الزامی است.")]
    public ProductRelationType RelationType { get; set; }

    public int SortOrder { get; set; } = 0;
}
