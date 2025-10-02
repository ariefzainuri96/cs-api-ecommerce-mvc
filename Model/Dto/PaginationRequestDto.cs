namespace Ecommerce.Model.Dto;

public class PaginationRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchAll { get; set; } = string.Empty;
    public string SearchField { get; set; } = string.Empty;
    public string SearchValue { get; set; } = string.Empty;
}
