public class UpsertProductColorDto
{
    public int Id { get; set; }
    public int ProductId { get; set; } // حتماً باید باشد
    public string ColorName { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
}
