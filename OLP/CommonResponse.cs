namespace OLP
{
    public class CommonResponse<T>
    {
        public CommonResponse()
        {
        }

        public CommonResponse(
            string message,
            T? result,
            bool isSuccess,
            int httpStatusCode)
        {
            Message = message;
            Result = result;
            IsSuccess = isSuccess;
            HttpStatusCode = httpStatusCode;
        }

        public string Message { get; set; } = string.Empty;
        public T? Result { get; set; }
        public bool IsSuccess { get; set; }
        public int HttpStatusCode { get; set; }

        public string? ErrorCode { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? TraceId { get; set; }
    }

    public static class CommonResponseMessage
    {
        public static string SuccessMessage { get; }
            = "Request processed successfully.";
    }
}
