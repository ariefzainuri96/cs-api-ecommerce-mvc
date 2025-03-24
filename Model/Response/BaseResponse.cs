namespace Ecommerce.Model.Response
{
    public class BaseResponse<T>
    {
        public required int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
