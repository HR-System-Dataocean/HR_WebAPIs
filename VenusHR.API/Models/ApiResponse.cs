namespace VenusHR.API.Models
{
    /// <summary>
    /// Unified API response model
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message (supports Arabic/English based on Lang parameter)
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The data returned by the API
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Error code (0 = success, non-zero = error)
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Additional error details (only populated on error)
        /// </summary>
        public string? ErrorDetails { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Create a successful response
        /// </summary>
        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                ErrorCode = 0
            };
        }

        /// <summary>
        /// Create an error response
        /// </summary>
        public static ApiResponse<T> Fail(string message, int errorCode = 1, string? errorDetails = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                ErrorDetails = errorDetails
            };
        }
    }

    /// <summary>
    /// Non-generic version for responses without data
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse Ok(string message = "Success")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                ErrorCode = 0
            };
        }

        public new static ApiResponse Fail(string message, int errorCode = 1, string? errorDetails = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                ErrorDetails = errorDetails
            };
        }
    }
}
