namespace Http.Client.Common
{
    public class BaseResponse<T>
    {
        public T? Data { get; set; }

        public string? ErrorMessage { get; set; }

        public BaseResponse(string errorMsg) 
        {
            ErrorMessage = errorMsg;
        }

        public BaseResponse()
        {
        }

        public BaseResponse(T data)
        {
            Data = data;
        }

        public bool IsSusses 
        { 
            get
            {
                return string.IsNullOrWhiteSpace(ErrorMessage);
            }
        }
    }
}
